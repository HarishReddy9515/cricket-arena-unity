using System;

namespace CricketArena.Core
{
    [Serializable]
    public struct PlayerLoadout
    {
        public string SquadName;
        public string SquadTag;
        public string Bat;
        public string Kit;
        public string Boost;
        public int Rating;
        public int Xp;
        public int Coins;

        public static PlayerLoadout Default => new PlayerLoadout
        {
            SquadName = "Harish XI",
            SquadTag = "Powerplay Squad",
            Bat = "Balanced",
            Kit = "Blue Steel",
            Boost = "Timing",
            Rating = 72,
            Xp = 0,
            Coins = 0
        };
    }
}
