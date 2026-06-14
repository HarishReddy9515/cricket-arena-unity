using CricketArena.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.UI
{
    public sealed class GameModeMenuController : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private CareerProgressionManager career;
        [SerializeField] private TournamentManager tournament;
        [SerializeField] private Text modeText;

        public void StartQuickMatch()
        {
            matchManager?.Configure(GameModeConfig.QuickMatch);
            matchManager?.StartChase();
            SetMode("Quick Match");
        }

        public void StartPracticeNets()
        {
            matchManager?.Configure(GameModeConfig.PracticeNets);
            matchManager?.StartChase();
            SetMode("Practice Nets");
        }

        public void StartCareer()
        {
            career?.StartCareerMatch();
            SetMode($"Career Level {career?.Level ?? 1}");
        }

        public void StartTournament()
        {
            tournament?.StartTournament();
            SetMode("Tournament");
        }

        public void StartOnlineRoom()
        {
            matchManager?.Configure(new GameModeConfig
            {
                Mode = CricketGameMode.OnlineRoom,
                DisplayName = "Online Room",
                TargetRuns = 24,
                MaxBalls = 6,
                MaxWickets = 2,
                AiDifficulty = 0.65f
            });
            matchManager?.StartChase();
            SetMode("Online Room");
        }

        private void SetMode(string value)
        {
            if (modeText != null)
            {
                modeText.text = value;
            }
        }
    }
}
