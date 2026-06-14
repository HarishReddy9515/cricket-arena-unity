using System;
using CricketArena.Presentation;

namespace CricketArena.Core
{
    [Serializable]
    public sealed class SaveGameData
    {
        public PlayerLoadout Loadout;
        public string SeasonName;
        public int SeasonTier;
        public int SeasonXp;
        public MobilePerformanceTier PerformanceTier;

        public static SaveGameData Default => new SaveGameData
        {
            Loadout = PlayerLoadout.Default,
            SeasonName = "Night League",
            SeasonTier = 1,
            SeasonXp = 0,
            PerformanceTier = MobilePerformanceTier.Balanced
        };
    }
}
