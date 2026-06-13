using System;
using System.Collections.Generic;
using UnityEngine;

namespace CricketArena.Networking
{
    public sealed class RealtimeMatchClient : MonoBehaviour
    {
        [SerializeField] private string serverUrl = "ws://localhost:8787";

        public bool IsConnected { get; private set; }
        public string RoomCode { get; private set; }

        public event Action<string> OnMessage;

        public void Connect()
        {
            // Replace this scaffold with NativeWebSocket, Mirror, Netcode for GameObjects,
            // or a custom authoritative server transport.
            IsConnected = true;
            OnMessage?.Invoke($"Connected to {serverUrl}");
        }

        public void JoinRoom(string roomCode)
        {
            RoomCode = string.IsNullOrWhiteSpace(roomCode) ? "ARENA-24" : roomCode;
            Send("join_room", new Dictionary<string, object> { ["room"] = RoomCode });
        }

        public void SendShot(float timing, string intent)
        {
            Send("shot", new Dictionary<string, object>
            {
                ["room"] = RoomCode,
                ["timing"] = timing,
                ["intent"] = intent
            });
        }

        private void Send(string type, Dictionary<string, object> payload)
        {
            if (!IsConnected) return;
            OnMessage?.Invoke($"{type}: {payload.Count} fields");
        }
    }
}
