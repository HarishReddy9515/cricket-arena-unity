using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Core
{
    public sealed class TournamentManager : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private int totalRounds = 4;
        [SerializeField] private int currentRound = 1;
        [SerializeField] private int wins;

        public UnityEvent<int, int, int> OnTournamentChanged;

        public int CurrentRound => currentRound;
        public int Wins => wins;
        public int TotalRounds => totalRounds;

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

        public void StartTournament()
        {
            currentRound = 1;
            wins = 0;
            StartRound();
        }

        public void StartRound()
        {
            matchManager?.Configure(GameModeConfig.TournamentChase(currentRound));
            matchManager?.StartChase();
            Publish();
        }

        private void OnScoreChanged(int runs, int wickets, int balls, int target)
        {
            if (matchManager == null || matchManager.Mode != CricketGameMode.TournamentChase || matchManager.Phase != MatchPhase.InningsComplete)
            {
                return;
            }

            if (runs >= target)
            {
                wins += 1;
                currentRound = Mathf.Min(totalRounds, currentRound + 1);
            }

            Publish();
        }

        private void Publish()
        {
            OnTournamentChanged?.Invoke(currentRound, wins, totalRounds);
        }
    }
}
