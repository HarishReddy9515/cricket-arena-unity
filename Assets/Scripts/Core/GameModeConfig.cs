using System;

namespace CricketArena.Core
{
    public enum CricketGameMode
    {
        QuickMatch,
        PracticeNets,
        CareerChase,
        TournamentChase,
        OnlineRoom
    }

    [Serializable]
    public struct GameModeConfig
    {
        public CricketGameMode Mode;
        public string DisplayName;
        public int TargetRuns;
        public int MaxBalls;
        public int MaxWickets;
        public float AiDifficulty;

        public static GameModeConfig QuickMatch => new GameModeConfig
        {
            Mode = CricketGameMode.QuickMatch,
            DisplayName = "Quick Match",
            TargetRuns = 24,
            MaxBalls = 6,
            MaxWickets = 2,
            AiDifficulty = 0.55f
        };

        public static GameModeConfig PracticeNets => new GameModeConfig
        {
            Mode = CricketGameMode.PracticeNets,
            DisplayName = "Practice Nets",
            TargetRuns = 999,
            MaxBalls = 18,
            MaxWickets = 99,
            AiDifficulty = 0.35f
        };

        public static GameModeConfig CareerChase(int level)
        {
            int safeLevel = Math.Max(1, level);
            return new GameModeConfig
            {
                Mode = CricketGameMode.CareerChase,
                DisplayName = $"Career Level {safeLevel}",
                TargetRuns = 18 + safeLevel * 4,
                MaxBalls = 6,
                MaxWickets = 2,
                AiDifficulty = Math.Min(0.95f, 0.45f + safeLevel * 0.06f)
            };
        }

        public static GameModeConfig TournamentChase(int round)
        {
            int safeRound = Math.Max(1, round);
            return new GameModeConfig
            {
                Mode = CricketGameMode.TournamentChase,
                DisplayName = $"Tournament Round {safeRound}",
                TargetRuns = 22 + safeRound * 5,
                MaxBalls = 6,
                MaxWickets = 2,
                AiDifficulty = Math.Min(1f, 0.52f + safeRound * 0.08f)
            };
        }
    }
}
