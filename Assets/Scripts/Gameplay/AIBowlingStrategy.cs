using CricketArena.Core;
using UnityEngine;

namespace CricketArena.Gameplay
{
    public sealed class AIBowlingStrategy : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;

        public DeliveryProfile Choose(DeliveryProfile[] deliveries)
        {
            if (deliveries == null || deliveries.Length == 0)
            {
                return DeliveryProfile.Default;
            }

            if (matchManager == null)
            {
                return deliveries[Random.Range(0, deliveries.Length)];
            }

            float pressure = Pressure();
            float targetDifficulty = Mathf.Clamp01(matchManager.AiDifficulty + pressure * 0.25f);
            DeliveryProfile best = deliveries[0];
            float bestScore = float.MaxValue;

            for (int i = 0; i < deliveries.Length; i++)
            {
                float score = Mathf.Abs(deliveries[i].Difficulty - targetDifficulty);
                score -= Random.Range(0f, 0.12f);
                if (score < bestScore)
                {
                    bestScore = score;
                    best = deliveries[i];
                }
            }

            return best;
        }

        private float Pressure()
        {
            int remainingRuns = Mathf.Max(0, matchManager.TargetRuns - matchManager.Runs);
            int remainingBalls = Mathf.Max(1, matchManager.MaxBalls - matchManager.Balls);
            float requiredRate = remainingRuns / (float)remainingBalls;
            float wicketPressure = matchManager.Wickets / (float)Mathf.Max(1, matchManager.MaxWickets);
            float chasePressure = Mathf.InverseLerp(2f, 8f, requiredRate);
            return Mathf.Clamp01(chasePressure * 0.7f + wicketPressure * 0.3f);
        }
    }
}
