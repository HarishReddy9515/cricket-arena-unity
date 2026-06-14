using CricketArena.Core;
using UnityEngine;

namespace CricketArena.UI
{
    public sealed class SeasonRewardBridge : MonoBehaviour
    {
        [SerializeField] private SeasonProgression seasonProgression;
        [SerializeField] private LoadoutController loadoutController;

        private void Awake()
        {
            if (seasonProgression != null)
            {
                seasonProgression.OnMissionCompleted.AddListener(OnMissionCompleted);
            }
        }

        private void OnDestroy()
        {
            if (seasonProgression != null)
            {
                seasonProgression.OnMissionCompleted.RemoveListener(OnMissionCompleted);
            }
        }

        private void OnMissionCompleted(int xp, int coins)
        {
            loadoutController?.AddReward(xp, coins);
        }
    }
}
