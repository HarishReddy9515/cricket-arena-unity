using CricketArena.Core;
using UnityEngine;

namespace CricketArena.Gameplay
{
    public sealed class BowlingController : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private BallPhysicsController ballPhysics;
        [SerializeField] private Animator bowlerAnimator;
        [SerializeField] private Transform releasePoint;
        [SerializeField] private Transform targetPoint;

        [Header("AI Bowling")]
        [SerializeField] private DeliveryProfile[] deliveries;

        public void BowlNext()
        {
            if (matchManager == null || ballPhysics == null || matchManager.Phase != MatchPhase.WaitingForDelivery)
            {
                return;
            }

            DeliveryProfile delivery = ChooseDelivery();
            bowlerAnimator?.SetTrigger("Bowl");
            ballPhysics.Launch(releasePoint.position, targetPoint.position, delivery);
            matchManager.MarkDeliveryLive();
        }

        private DeliveryProfile ChooseDelivery()
        {
            if (deliveries == null || deliveries.Length == 0)
            {
                return DeliveryProfile.Default;
            }

            int need = matchManager.TargetRuns - matchManager.Runs;
            if (need <= 6)
            {
                return deliveries[0];
            }

            return deliveries[Random.Range(0, deliveries.Length)];
        }
    }
}
