using CricketArena.Core;
using CricketArena.Gameplay;
using UnityEngine;

namespace CricketArena.Networking
{
    public sealed class NetworkGameplaySynchronizer : MonoBehaviour
    {
        [SerializeField] private RealtimeMatchClient client;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private BowlingController bowlingController;

        private void Awake()
        {
            if (client == null) return;
            client.OnDelivery += HandleDelivery;
            client.OnMatchState += HandleMatchState;
        }

        private void Start()
        {
            matchManager?.StartChase();
        }

        private void OnDestroy()
        {
            if (client == null) return;
            client.OnDelivery -= HandleDelivery;
            client.OnMatchState -= HandleMatchState;
        }

        private void HandleDelivery(DeliveryMessage message)
        {
            if (message?.delivery == null || bowlingController == null) return;
            bowlingController.BowlServerDelivery(ToProfile(message.delivery));
        }

        private void HandleMatchState(DeliveryMessage message)
        {
            if (message?.room == null || matchManager == null) return;

            string status = message.room.status;
            string resultMessage = message.outcome != null ? message.outcome.message : $"Server: {status}";
            matchManager.SyncAuthoritativeState(
                message.room.score,
                message.room.wickets,
                message.room.balls,
                status,
                resultMessage
            );
        }

        private static DeliveryProfile ToProfile(DeliveryDto dto)
        {
            return new DeliveryProfile
            {
                Name = dto.name,
                Speed = dto.speed,
                Swing = dto.swing,
                Bounce = dto.bounce,
                Difficulty = dto.difficulty
            };
        }
    }
}
