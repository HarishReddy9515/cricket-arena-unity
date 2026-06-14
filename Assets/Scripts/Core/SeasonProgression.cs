using System;
using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Core
{
    [Serializable]
    public struct SeasonMission
    {
        public string Id;
        public string Title;
        public int Target;
        public int Progress;
        public int XpReward;
        public int CoinReward;

        public bool IsComplete => Progress >= Target;
    }

    public sealed class SeasonProgression : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private string seasonName = "Night League";
        [SerializeField] private int tier = 1;
        [SerializeField] private int seasonXp;
        [SerializeField] private SeasonMission[] missions =
        {
            new SeasonMission { Id = "score-runs", Title = "Score 30 runs", Target = 30, XpReward = 120, CoinReward = 40 },
            new SeasonMission { Id = "hit-boundaries", Title = "Hit 3 boundaries", Target = 3, XpReward = 90, CoinReward = 25 },
            new SeasonMission { Id = "win-chases", Title = "Win 2 chases", Target = 2, XpReward = 180, CoinReward = 60 }
        };

        public UnityEvent<string, int, int> OnSeasonChanged;

        public string SeasonName => seasonName;
        public int Tier => tier;
        public int SeasonXp => seasonXp;
        public SeasonMission[] Missions => missions;

        private void Awake()
        {
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

        public void AddRuns(int runs)
        {
            AddMissionProgress("score-runs", runs);
        }

        public void AddBoundary()
        {
            AddMissionProgress("hit-boundaries", 1);
        }

        public void AddWin()
        {
            AddMissionProgress("win-chases", 1);
        }

        private void AddMissionProgress(string missionId, int amount)
        {
            for (int i = 0; i < missions.Length; i++)
            {
                if (missions[i].Id != missionId || missions[i].IsComplete) continue;

                missions[i].Progress = Mathf.Min(missions[i].Target, missions[i].Progress + Mathf.Max(0, amount));
                if (missions[i].IsComplete)
                {
                    seasonXp += missions[i].XpReward;
                    tier = Mathf.Max(1, 1 + seasonXp / 250);
                }
                Publish();
                return;
            }
        }

        private void OnScoreChanged(int runs, int wickets, int balls, int target)
        {
            if (matchManager == null || matchManager.Phase != MatchPhase.InningsComplete) return;
            if (runs >= target)
            {
                AddWin();
            }
        }

        private void Publish()
        {
            OnSeasonChanged?.Invoke(seasonName, tier, seasonXp);
        }
    }
}
