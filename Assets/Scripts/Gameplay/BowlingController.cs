using CricketArena.Core;
using CricketArena.Presentation;
using UnityEngine;

namespace CricketArena.Gameplay
{
    public sealed class BowlingController : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private BallPhysicsController ballPhysics;
        [SerializeField] private Animator bowlerAnimator;
        [SerializeField] private PlayerAnimationDirector animationDirector;
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
            PlayBowlingAnimation(delivery);
            ballPhysics.Launch(releasePoint.position, targetPoint.position, delivery);
            matchManager.MarkDeliveryLive();
        }

        public void BowlServerDelivery(DeliveryProfile delivery)
        {
            if (matchManager == null || ballPhysics == null || matchManager.Phase == MatchPhase.InningsComplete)
            {
                return;
            }

            PlayBowlingAnimation(delivery);
            ballPhysics.Launch(releasePoint.position, targetPoint.position, delivery);
            matchManager.MarkDeliveryLive();
        }

        private void PlayBowlingAnimation(DeliveryProfile delivery)
        {
            animationDirector?.PlayBowling(delivery);
            if (animationDirector == null)
            {
                bowlerAnimator?.SetTrigger("Bowl");
            }
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
