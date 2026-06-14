using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Core
{
    public sealed class CareerProgressionManager : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private int level = 1;
        [SerializeField] private int skillPoints;

        public UnityEvent<int, int> OnCareerChanged;

        public int Level => level;
        public int SkillPoints => skillPoints;

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

        public void StartCareerMatch()
        {
            matchManager?.Configure(GameModeConfig.CareerChase(level));
            matchManager?.StartChase();
            Publish();
        }

        public void ResetCareer()
        {
            level = 1;
            skillPoints = 0;
            Publish();
        }

        private void OnScoreChanged(int runs, int wickets, int balls, int target)
        {
            if (matchManager == null || matchManager.Mode != CricketGameMode.CareerChase || matchManager.Phase != MatchPhase.InningsComplete)
            {
                return;
            }

            if (runs >= target)
            {
                level += 1;
                skillPoints += Mathf.Max(1, 3 - wickets);
                Publish();
            }
        }

        private void Publish()
        {
            OnCareerChanged?.Invoke(level, skillPoints);
        }
    }
}
