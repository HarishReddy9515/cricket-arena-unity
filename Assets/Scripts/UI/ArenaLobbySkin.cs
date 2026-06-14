using CricketArena.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.UI
{
    public sealed class ArenaLobbySkin : MonoBehaviour
    {
        [SerializeField] private Color panelColor = new Color(0.03f, 0.04f, 0.05f, 0.82f);
        [SerializeField] private Color accentColor = new Color(1f, 0.72f, 0.18f, 0.95f);
        [SerializeField] private Color secondaryColor = new Color(0.08f, 0.36f, 0.62f, 0.92f);
        [SerializeField] private Text titleText;
        [SerializeField] private Text seasonText;
        [SerializeField] private Text currencyText;
        [SerializeField] private Text squadText;
        [SerializeField] private Text loadoutText;
        [SerializeField] private Text primaryActionText;

        private PlayerLoadout loadout = PlayerLoadout.Default;

        public void SetLoadout(PlayerLoadout value)
        {
            loadout = value;
            Apply();
        }

        public void Apply()
        {
            Image[] images = GetComponentsInChildren<Image>(true);
            foreach (Image image in images)
            {
                if (image.gameObject.name.Contains("Primary"))
                {
                    image.color = accentColor;
                }
                else if (image.gameObject.name.Contains("Mode") || image.gameObject.name.Contains("Connect"))
                {
                    image.color = secondaryColor;
                }
                else if (image.gameObject.name.Contains("Panel") || image.gameObject.name.Contains("Bar"))
                {
                    image.color = panelColor;
                }
            }

            Set(titleText, "CRICKET ARENA");
            Set(seasonText, "Season 01 | Night League");
            Set(currencyText, $"XP {loadout.Xp}  |  Coins {loadout.Coins}");
            Set(squadText, $"{loadout.SquadName}\n{loadout.SquadTag}\nRating {loadout.Rating}");
            Set(loadoutText, $"Bat: {loadout.Bat}\nKit: {loadout.Kit}\nBoost: {loadout.Boost}");
            Set(primaryActionText, "PLAY");
        }

        private void Awake()
        {
            Apply();
        }

        private static void Set(Text text, string value)
        {
            if (text != null)
            {
                text.text = value;
            }
        }
    }
}
