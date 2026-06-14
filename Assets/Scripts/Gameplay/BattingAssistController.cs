using CricketArena.Core;
using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Gameplay
{
    public sealed class BattingAssistController : MonoBehaviour
    {
        [SerializeField] private SeasonProgression seasonProgression;
        [SerializeField] private bool timingAssistEnabled = true;
        [SerializeField] private float assistStrength = 0.08f;

        public UnityEvent<string> OnTimingFeedback;

        public float ApplyAssist(float quality, ShotIntent intent)
        {
            if (!timingAssistEnabled || intent == ShotIntent.Defensive)
            {
                return quality;
            }

            return Mathf.Clamp01(quality + assistStrength * (1f - quality));
        }

        public void ReportOutcome(ShotOutcome outcome, float quality)
        {
            seasonProgression?.AddRuns(outcome.Runs);
            if (outcome.Runs >= 4)
            {
                seasonProgression?.AddBoundary();
            }

            OnTimingFeedback?.Invoke(FeedbackFor(quality, outcome));
        }

        private static string FeedbackFor(float quality, ShotOutcome outcome)
        {
            if (outcome.IsWicket) return "Late";
            if (quality >= 0.9f) return "Perfect";
            if (quality >= 0.7f) return "Great";
            if (quality >= 0.45f) return "Good";
            return "Early";
        }
    }
}
