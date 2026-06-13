using CricketArena.Core;
using CricketArena.Presentation;
using UnityEngine;

namespace CricketArena.Gameplay
{
    public enum ShotIntent
    {
        Straight,
        LoftLeft,
        CutRight,
        Defensive
    }

    public sealed class BattingController : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private BallPhysicsController ballPhysics;
        [SerializeField] private Animator batterAnimator;
        [SerializeField] private MobileHaptics haptics;

        [Header("Timing")]
        [SerializeField] private float perfectContactDistance = 1.25f;
        [SerializeField] private Transform contactPoint;

        private ShotIntent intent = ShotIntent.Straight;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.A)) intent = ShotIntent.LoftLeft;
            if (Input.GetKeyDown(KeyCode.D)) intent = ShotIntent.CutRight;
            if (Input.GetKeyDown(KeyCode.S)) intent = ShotIntent.Defensive;
            if (Input.GetKeyDown(KeyCode.Space)) PlayShot();
        }

        public void SetIntent(int intentIndex)
        {
            intent = (ShotIntent)intentIndex;
        }

        public void PlayShot()
        {
            if (matchManager == null || ballPhysics == null || matchManager.Phase != MatchPhase.DeliveryLive)
            {
                return;
            }

            float distance = Vector3.Distance(ballPhysics.transform.position, contactPoint.position);
            float contactQuality = Mathf.Clamp01(1f - distance / perfectContactDistance);
            float timingQuality = ballPhysics.NormalizedTimingWindow;
            float quality = Mathf.Clamp01(contactQuality * 0.62f + timingQuality * 0.38f);

            ShotOutcome outcome = ResolveOutcome(quality);
            batterAnimator?.SetTrigger(intent == ShotIntent.Defensive ? "Defend" : "Hit");
            ballPhysics.ResolveShot(outcome.Power, DirectionForIntent(intent));
            haptics?.PlayShotFeedback(outcome);
            matchManager.ApplyOutcome(outcome);
            intent = ShotIntent.Straight;
        }

        private ShotOutcome ResolveOutcome(float quality)
        {
            if (quality < 0.18f) return new ShotOutcome(0, true, "Caught behind", 0.1f);
            if (quality < 0.36f) return new ShotOutcome(0, false, "Beaten", 0.2f);
            if (quality < 0.55f) return new ShotOutcome(1, false, "Quick single", 0.45f);
            if (quality < 0.74f) return new ShotOutcome(2, false, "Two runs", 0.62f);
            if (quality < 0.9f) return new ShotOutcome(4, false, "Four", 0.82f);
            return new ShotOutcome(6, false, "Six", 1f);
        }

        private Vector3 DirectionForIntent(ShotIntent shotIntent)
        {
            return shotIntent switch
            {
                ShotIntent.LoftLeft => new Vector3(-0.45f, 0.5f, 1f).normalized,
                ShotIntent.CutRight => new Vector3(0.45f, 0.35f, 1f).normalized,
                ShotIntent.Defensive => new Vector3(0f, 0.08f, 1f).normalized,
                _ => new Vector3(0f, 0.45f, 1f).normalized
            };
        }
    }
}
