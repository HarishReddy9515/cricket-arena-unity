using CricketArena.Core;
using UnityEngine;

namespace CricketArena.UI
{
    public sealed class LoadoutController : MonoBehaviour
    {
        [SerializeField] private ArenaLobbySkin lobbySkin;

        private readonly string[] bats = { "Balanced", "Power", "Control" };
        private readonly string[] kits = { "Blue Steel", "Night Gold", "Arena White" };
        private readonly string[] boosts = { "Timing", "Powerplay", "Placement" };
        private PlayerLoadout loadout = PlayerLoadout.Default;
        private int batIndex;
        private int kitIndex;
        private int boostIndex;

        private void Awake()
        {
            Publish();
        }

        public void NextBat()
        {
            batIndex = (batIndex + 1) % bats.Length;
            loadout.Bat = bats[batIndex];
            Publish();
        }

        public void NextKit()
        {
            kitIndex = (kitIndex + 1) % kits.Length;
            loadout.Kit = kits[kitIndex];
            Publish();
        }

        public void NextBoost()
        {
            boostIndex = (boostIndex + 1) % boosts.Length;
            loadout.Boost = boosts[boostIndex];
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
