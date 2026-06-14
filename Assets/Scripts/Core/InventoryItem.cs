using System;

namespace CricketArena.Core
{
    public enum InventoryItemType
    {
        Bat,
        Kit,
        Boost,
        Banner
    }

    [Serializable]
    public struct InventoryItem
    {
        public string Id;
        public string DisplayName;
        public InventoryItemType Type;
        public int RatingBonus;
        public bool Unlocked;
    }
}
