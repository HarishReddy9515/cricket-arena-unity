using System;
using UnityEngine;

namespace CricketArena.Networking
{
    public sealed class RealtimeMatchClient : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "ws://localhost:8790";
        [SerializeField] private string defaultRoomCode = "ARENA-24";

        public bool IsConnected { get; private set; }
        public string RoomCode { get; private set; }
        public string LastOutboundJson { get; private set; }
        public string LastInboundJson { get; private set; }

        public event Action<string> OnMessage;
        public event Action<DeliveryMessage> OnDelivery;
        public event Action<DeliveryMessage> OnMatchState;

        public void Connect()
        {
            // Transport hook: send LastOutboundJson through NativeWebSocket, Mirror,
            // Netcode for GameObjects, or a mobile platform WebSocket plugin.
            IsConnected = true;
            OnMessage?.Invoke($"Connected to {serverUrl}");
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
            if (envelope.type == MatchEvents.Delivery)
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
        }
    }
}
