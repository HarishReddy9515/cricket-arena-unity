using CricketArena.Core;
using UnityEngine;

namespace CricketArena.Presentation
{
    public sealed class MobileHaptics : MonoBehaviour
    {
        [SerializeField] private bool enableHaptics = true;

        public void PlayShotFeedback(ShotOutcome outcome)
        {
            if (!enableHaptics) return;

            if (outcome.IsWicket)
            {
                Handheld.Vibrate();
                return;
            }

            if (outcome.Runs >= 4)
            {
                Handheld.Vibrate();
            }
        }
    }
}
