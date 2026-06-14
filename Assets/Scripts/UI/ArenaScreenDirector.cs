using CricketArena.Presentation;
using UnityEngine;

namespace CricketArena.UI
{
    public sealed class ArenaScreenDirector : MonoBehaviour
    {
        [SerializeField] private CameraDirector cameraDirector;
        [SerializeField] private CanvasGroup[] lobbyGroups;
        [SerializeField] private CanvasGroup[] gameplayGroups;

        private void Awake()
        {
            ShowLobby();
        }

        public void ShowLobby()
        {
            SetGroups(lobbyGroups, true);
            SetGroups(gameplayGroups, false);
            cameraDirector?.ShowLobbyCamera();
        }

        public void ShowGameplay()
        {
            SetGroups(lobbyGroups, false);
            SetGroups(gameplayGroups, true);
            cameraDirector?.ShowBattingCamera();
        }

        private static void SetGroups(CanvasGroup[] groups, bool visible)
        {
            if (groups == null) return;

            foreach (CanvasGroup group in groups)
            {
                if (group == null) continue;
                group.alpha = visible ? 1f : 0f;
                group.interactable = visible;
                group.blocksRaycasts = visible;
            }
        }
    }
}
