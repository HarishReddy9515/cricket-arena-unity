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
        [SerializeField] private ArenaScreenDirector screenDirector;
        [SerializeField] private Text modeText;

        public void StartQuickMatch()
        {
            matchManager?.Configure(GameModeConfig.QuickMatch);
            matchManager?.StartChase();
            screenDirector?.ShowGameplay();
            SetMode("Quick Match");
        }

        public void StartPracticeNets()
        {
            matchManager?.Configure(GameModeConfig.PracticeNets);
            matchManager?.StartChase();
            screenDirector?.ShowGameplay();
            SetMode("Practice Nets");
        }

        public void StartCareer()
        {
            career?.StartCareerMatch();
            screenDirector?.ShowGameplay();
            SetMode($"Career Level {career?.Level ?? 1}");
        }

        public void StartTournament()
        {
            tournament?.StartTournament();
            screenDirector?.ShowGameplay();
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
            screenDirector?.ShowGameplay();
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
