using CricketArena.Core;
using CricketArena.Gameplay;
using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class PlayerAnimationDirector : MonoBehaviour
    {
        private static readonly int BowlTrigger = Animator.StringToHash("Bowl");
        private static readonly int HitTrigger = Animator.StringToHash("Hit");
        private static readonly int DefendTrigger = Animator.StringToHash("Defend");
        private static readonly int WicketTrigger = Animator.StringToHash("Wicket");
        private static readonly int CelebrateTrigger = Animator.StringToHash("Celebrate");
        private static readonly int ShotPower = Animator.StringToHash("ShotPower");
        private static readonly int ShotIntentParam = Animator.StringToHash("ShotIntent");
        private static readonly int DeliverySpeed = Animator.StringToHash("DeliverySpeed");
        private static readonly int MatchPhaseParam = Animator.StringToHash("MatchPhase");

        [SerializeField] private Animator batterAnimator;
        [SerializeField] private Animator bowlerAnimator;
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CricketAssetManifest manifest;

        private void Awake()
        {
            ApplyManifestControllers();
            if (matchManager != null)
            {
                matchManager.OnScoreChanged.AddListener(OnScoreChanged);
            }
        }

        private void OnDestroy()
        {
            if (matchManager != null)
            {
                matchManager.OnScoreChanged.RemoveListener(OnScoreChanged);
            }
        }

        public void ApplyManifestControllers()
        {
            if (manifest == null) return;

            if (batterAnimator != null && manifest.batterAnimator != null)
            {
                batterAnimator.runtimeAnimatorController = manifest.batterAnimator;
            }

            if (bowlerAnimator != null && manifest.bowlerAnimator != null)
            {
                bowlerAnimator.runtimeAnimatorController = manifest.bowlerAnimator;
            }
        }

        public void PlayBowling(DeliveryProfile delivery)
        {
            if (bowlerAnimator == null) return;

            bowlerAnimator.SetFloat(DeliverySpeed, Mathf.Clamp01(delivery.Speed / 36f));
            bowlerAnimator.SetTrigger(BowlTrigger);
        }

        public void PlayShot(ShotIntent intent, ShotOutcome outcome)
        {
            if (batterAnimator == null) return;

            batterAnimator.SetInteger(ShotIntentParam, (int)intent);
            batterAnimator.SetFloat(ShotPower, outcome.Power);

            if (outcome.IsWicket)
            {
                batterAnimator.SetTrigger(WicketTrigger);
                return;
            }

            batterAnimator.SetTrigger(intent == ShotIntent.Defensive ? DefendTrigger : HitTrigger);
        }

        public void SyncPhase(MatchPhase phase)
        {
            batterAnimator?.SetInteger(MatchPhaseParam, (int)phase);
            bowlerAnimator?.SetInteger(MatchPhaseParam, (int)phase);
        }

        public void CelebrateBowler()
        {
            bowlerAnimator?.SetTrigger(CelebrateTrigger);
        }

        private void OnScoreChanged(int runs, int wickets, int balls, int target)
        {
            if (matchManager == null) return;
            SyncPhase(matchManager.Phase);
        }
    }
}
