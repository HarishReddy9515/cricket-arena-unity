using CricketArena.Core;
using UnityEngine;

namespace CricketArena.UI
{
    public sealed class LoadoutController : MonoBehaviour
    {
        [SerializeField] private ArenaLobbySkin lobbySkin;
        [SerializeField] private InventoryManager inventory;

        private PlayerLoadout loadout = PlayerLoadout.Default;

        private void Awake()
        {
            Publish();
        }

        public void NextBat()
        {
            InventoryItem item = inventory != null ? inventory.NextUnlocked(InventoryItemType.Bat, loadout.Bat) : default;
            if (!string.IsNullOrWhiteSpace(item.DisplayName))
            {
                loadout.Bat = item.DisplayName;
                loadout.Rating = PlayerLoadout.Default.Rating + item.RatingBonus;
            }
            Publish();
        }

        public void NextKit()
        {
            InventoryItem item = inventory != null ? inventory.NextUnlocked(InventoryItemType.Kit, loadout.Kit) : default;
            if (!string.IsNullOrWhiteSpace(item.DisplayName))
            {
                loadout.Kit = item.DisplayName;
                loadout.Rating = PlayerLoadout.Default.Rating + item.RatingBonus;
            }
            Publish();
        }

        public void NextBoost()
        {
            InventoryItem item = inventory != null ? inventory.NextUnlocked(InventoryItemType.Boost, loadout.Boost) : default;
            if (!string.IsNullOrWhiteSpace(item.DisplayName))
            {
                loadout.Boost = item.DisplayName;
                loadout.Rating = PlayerLoadout.Default.Rating + item.RatingBonus;
            }
            Publish();
        }

        public void AddReward(int xp, int coins)
        {
            loadout.Xp += Mathf.Max(0, xp);
            loadout.Coins += Mathf.Max(0, coins);
            Publish();
        }

        private void Publish()
        {
            lobbySkin?.SetLoadout(loadout);
        }
    }
}
