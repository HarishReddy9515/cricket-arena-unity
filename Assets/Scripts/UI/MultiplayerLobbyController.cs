using CricketArena.Gameplay;
using CricketArena.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.UI
{
    public sealed class MultiplayerLobbyController : MonoBehaviour
    {
        [SerializeField] private RealtimeMatchClient client;
        [SerializeField] private BattingController battingController;
        [SerializeField] private BowlingController bowlingController;
        [SerializeField] private InputField roomCodeInput;
        [SerializeField] private Text statusText;

        private string currentIntent = "straight";
        private float lastShotTiming = 0.5f;

        private void Awake()
        {
            if (client != null)
            {
                client.OnMessage += SetStatus;
                client.OnDelivery += OnDelivery;
                client.OnMatchState += OnMatchState;
                client.OnServerError += OnServerError;
            }
        }

        private void OnDestroy()
        {
            if (client != null)
            {
                client.OnMessage -= SetStatus;
                client.OnDelivery -= OnDelivery;
                client.OnMatchState -= OnMatchState;
                client.OnServerError -= OnServerError;
            }
        }

        public void Connect()
        {
            client?.Connect();
        }

        public void JoinRoom()
        {
            client?.JoinRoom(roomCodeInput != null ? roomCodeInput.text : string.Empty);
        }

        public void Ready()
        {
            client?.SetReady(true);
        }

        public void RequestDelivery()
        {
            client?.RequestDelivery();
            if (client == null || !client.IsConnected)
            {
                bowlingController?.BowlNext();
            }
        }

        public void SetStraightIntent()
        {
            currentIntent = "straight";
            battingController?.SetIntent((int)ShotIntent.Straight);
        }

        public void SetLoftLeftIntent()
        {
            currentIntent = "loftLeft";
            battingController?.SetIntent((int)ShotIntent.LoftLeft);
        }

        public void SetCutRightIntent()
        {
            currentIntent = "cutRight";
            battingController?.SetIntent((int)ShotIntent.CutRight);
        }

        public void SetShotTiming(float timing)
        {
            lastShotTiming = Mathf.Clamp01(timing);
        }

        public void SendShot()
        {
            client?.SendShot(lastShotTiming, currentIntent);
            if (client == null || !client.IsConnected)
            {
                battingController?.PlayShot();
            }
        }

        private void OnDelivery(DeliveryMessage message)
        {
            if (message?.delivery == null) return;
            SetStatus($"Delivery: {message.delivery.name}");
        }

        private void OnMatchState(DeliveryMessage message)
        {
            if (message?.room == null) return;
            SetStatus($"{message.room.code} {message.room.score}/{message.room.wickets} after {message.room.balls} | {client?.LastLatencyMs ?? 0}ms");
        }

        private void OnServerError(ErrorMessage message)
        {
            if (message == null) return;
            SetStatus($"{message.code}: {message.message}");
        }

        private void SetStatus(string value)
        {
            if (statusText != null) statusText.text = value;
        }
    }
}
