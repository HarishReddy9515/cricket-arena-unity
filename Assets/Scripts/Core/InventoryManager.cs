using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Core
{
    public sealed class InventoryManager : MonoBehaviour
    {
        [SerializeField] private InventoryItem[] items =
        {
            new InventoryItem { Id = "bat-balanced", DisplayName = "Balanced", Type = InventoryItemType.Bat, RatingBonus = 0, Unlocked = true },
            new InventoryItem { Id = "bat-power", DisplayName = "Power", Type = InventoryItemType.Bat, RatingBonus = 3, Unlocked = true },
            new InventoryItem { Id = "kit-blue-steel", DisplayName = "Blue Steel", Type = InventoryItemType.Kit, RatingBonus = 0, Unlocked = true },
            new InventoryItem { Id = "kit-night-gold", DisplayName = "Night Gold", Type = InventoryItemType.Kit, RatingBonus = 2, Unlocked = true },
            new InventoryItem { Id = "boost-timing", DisplayName = "Timing", Type = InventoryItemType.Boost, RatingBonus = 2, Unlocked = true },
            new InventoryItem { Id = "boost-placement", DisplayName = "Placement", Type = InventoryItemType.Boost, RatingBonus = 2, Unlocked = true }
        };

        public UnityEvent<InventoryItem[]> OnInventoryChanged;

        public InventoryItem[] Items => items;

        public InventoryItem NextUnlocked(InventoryItemType type, string currentDisplayName)
        {
            int start = 0;
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Type == type && items[i].DisplayName == currentDisplayName)
                {
                    start = i + 1;
                    break;
                }
            }

            for (int offset = 0; offset < items.Length; offset++)
            {
                InventoryItem item = items[(start + offset) % items.Length];
                if (item.Type == type && item.Unlocked)
                {
                    return item;
                }
            }

            return default;
        }

        public void Unlock(string id)
        {
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i].Id != id) continue;
                items[i].Unlocked = true;
                OnInventoryChanged?.Invoke(items);
                return;
            }
        }
    }
}
