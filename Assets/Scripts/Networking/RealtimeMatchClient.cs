using System;
using System.Collections.Concurrent;
#if !UNITY_WEBGL
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#endif
using UnityEngine;

namespace CricketArena.Networking
{
    public sealed class RealtimeMatchClient : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "ws://localhost:8790";
        [SerializeField] private string defaultRoomCode = "ARENA-24";
        [SerializeField] private bool autoReconnect = true;
        [SerializeField] private float heartbeatIntervalSeconds = 5f;
        [SerializeField] private float reconnectDelaySeconds = 2f;
        [SerializeField] private int maxReconnectAttempts = 5;

        public bool IsConnected { get; private set; }
        public string RoomCode { get; private set; }
        public string PlayerId { get; private set; }
        public string LastOutboundJson { get; private set; }
        public string LastInboundJson { get; private set; }
        public long LastLatencyMs { get; private set; }
        public int ReconnectAttempts { get; private set; }

        public event Action<string> OnMessage;
        public event Action<DeliveryMessage> OnDelivery;
        public event Action<DeliveryMessage> OnMatchState;
        public event Action<ErrorMessage> OnServerError;

#if !UNITY_WEBGL
        private ClientWebSocket socket;
        private CancellationTokenSource cancellation;
#endif
        private readonly ConcurrentQueue<string> inboundQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentQueue<string> statusQueue = new ConcurrentQueue<string>();
        private float nextHeartbeatAt;
        private float nextReconnectAt;
        private bool userRequestedDisconnect;

        private void Update()
        {
            if (IsConnected && Time.unscaledTime >= nextHeartbeatAt)
            {
                Ping();
            }

            if (!IsConnected && autoReconnect && !userRequestedDisconnect && ReconnectAttempts > 0 && Time.unscaledTime >= nextReconnectAt)
            {
                Connect();
            }

            while (statusQueue.TryDequeue(out string status))
            {
                OnMessage?.Invoke(status);
            }

            while (inboundQueue.TryDequeue(out string json))
            {
                ReceiveJson(json);
            }
        }

        private void OnDestroy()
        {
            Disconnect();
        }

        public async void Connect()
        {
            userRequestedDisconnect = false;
#if UNITY_WEBGL
            IsConnected = true;
            statusQueue.Enqueue("WebGL requires a browser WebSocket plugin transport.");
            Ping();
#else
            if (IsConnected) return;

            DisposeSocket();
            cancellation = new CancellationTokenSource();
            socket = new ClientWebSocket();

            try
            {
                await socket.ConnectAsync(new Uri(serverUrl), cancellation.Token);
                IsConnected = true;
                ReconnectAttempts = 0;
                nextHeartbeatAt = Time.unscaledTime + heartbeatIntervalSeconds;
                statusQueue.Enqueue($"Connected to {serverUrl}");
                _ = ReceiveLoop(cancellation.Token);
                Ping();
            }
            catch (Exception ex)
            {
                IsConnected = false;
                statusQueue.Enqueue($"Connection failed: {ex.Message}");
                DisposeSocket();
                ScheduleReconnect();
            }
#endif
        }

        public async void Disconnect()
        {
            userRequestedDisconnect = true;
#if !UNITY_WEBGL
            try
            {
                if (socket != null && socket.State == WebSocketState.Open)
                {
                    await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Client disconnect", CancellationToken.None);
                }
            }
            catch (Exception ex)
            {
                statusQueue.Enqueue($"Disconnect error: {ex.Message}");
            }
            finally
            {
                DisposeSocket();
            }
#else
            IsConnected = false;
#endif
        }

        public void Ping()
        {
            nextHeartbeatAt = Time.unscaledTime + heartbeatIntervalSeconds;
            Send(MatchMessage.Ping(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()));
        }

        public void JoinRoom(string roomCode)
        {
            RoomCode = string.IsNullOrWhiteSpace(roomCode) ? defaultRoomCode : roomCode.Trim().ToUpperInvariant();
            Send(MatchMessage.JoinRoom(RoomCode));
        }

        public void SetReady(bool ready)
        {
            Send(MatchMessage.Ready(ready));
        }

        public void RequestDelivery()
        {
            Send(MatchMessage.RequestDelivery());
        }

        public void SendShot(float timing, string intent)
        {
            Send(MatchMessage.Shot(Mathf.Clamp01(timing), intent));
        }

        public void ReceiveJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return;
            LastInboundJson = json;
            MatchMessage envelope = JsonUtility.FromJson<MatchMessage>(json);
            if (envelope == null || string.IsNullOrWhiteSpace(envelope.type)) return;

            OnMessage?.Invoke($"server:{envelope.type}");
            if (envelope.type == MatchEvents.Connected)
            {
                PlayerId = envelope.playerId;
                OnMessage?.Invoke($"player:{PlayerId}");
            }
            else if (envelope.type == MatchEvents.Pong)
            {
                LastLatencyMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - envelope.clientTime;
                OnMessage?.Invoke($"latency:{LastLatencyMs}ms");
            }
            else if (envelope.type == MatchEvents.Error)
            {
                OnServerError?.Invoke(JsonUtility.FromJson<ErrorMessage>(json));
            }
            else if (envelope.type == MatchEvents.Delivery)
            {
                OnDelivery?.Invoke(JsonUtility.FromJson<DeliveryMessage>(json));
            }
            else if (envelope.type == MatchEvents.MatchState || envelope.type == MatchEvents.RoomState)
            {
                OnMatchState?.Invoke(JsonUtility.FromJson<DeliveryMessage>(json));
            }
        }

        private void Send(MatchMessage message)
        {
            if (!IsConnected) return;
            LastOutboundJson = JsonUtility.ToJson(message);
            OnMessage?.Invoke($"client:{message.type}");
#if !UNITY_WEBGL
            _ = SendJsonAsync(LastOutboundJson);
#endif
        }

#if !UNITY_WEBGL
        private async Task SendJsonAsync(string json)
        {
            if (socket == null || socket.State != WebSocketState.Open) return;

            byte[] bytes = Encoding.UTF8.GetBytes(json);
            try
            {
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, cancellation.Token);
            }
            catch (Exception ex)
            {
                statusQueue.Enqueue($"Send failed: {ex.Message}");
            }
        }

        private async Task ReceiveLoop(CancellationToken token)
        {
            byte[] buffer = new byte[8192];
            StringBuilder builder = new StringBuilder();

            while (!token.IsCancellationRequested && socket != null && socket.State == WebSocketState.Open)
            {
                try
                {
                    WebSocketReceiveResult result = await socket.ReceiveAsync(new ArraySegment<byte>(buffer), token);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        IsConnected = false;
                        statusQueue.Enqueue("Server closed connection.");
                        ScheduleReconnect();
                        return;
                    }

                    builder.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
                    if (result.EndOfMessage)
                    {
                        inboundQueue.Enqueue(builder.ToString());
                        builder.Clear();
                    }
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    statusQueue.Enqueue($"Receive failed: {ex.Message}");
                    ScheduleReconnect();
                    return;
                }
            }
        }

        private void ScheduleReconnect()
        {
            if (!autoReconnect || userRequestedDisconnect || ReconnectAttempts >= maxReconnectAttempts) return;
            ReconnectAttempts += 1;
            nextReconnectAt = Time.unscaledTime + reconnectDelaySeconds * ReconnectAttempts;
            statusQueue.Enqueue($"Reconnect scheduled ({ReconnectAttempts}/{maxReconnectAttempts})");
        }

        private void DisposeSocket()
        {
            IsConnected = false;
            cancellation?.Cancel();
            socket?.Dispose();
            cancellation?.Dispose();
            socket = null;
            cancellation = null;
        }
#endif
    }
}
