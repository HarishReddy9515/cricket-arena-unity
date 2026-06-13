using CricketArena.Core;
using UnityEngine;
using UnityEngine.UI;

namespace CricketArena.UI
{
    public sealed class ScoreHudController : MonoBehaviour
    {
        [SerializeField] private MatchManager matchManager;
        [SerializeField] private Text scoreText;
        [SerializeField] private Text equationText;
        [SerializeField] private Text messageText;

        private void OnEnable()
        {
            if (matchManager == null) return;
            matchManager.OnScoreChanged.AddListener(UpdateScore);
            matchManager.OnMessage.AddListener(UpdateMessage);
        }

        private void OnDisable()
        {
            if (matchManager == null) return;
            matchManager.OnScoreChanged.RemoveListener(UpdateScore);
            matchManager.OnMessage.RemoveListener(UpdateMessage);
        }

        public void Bind(MatchManager manager)
        {
            matchManager = manager;
        }

        public void BindText(Text score, Text equation, Text message)
        {
            scoreText = score;
            equationText = equation;
            messageText = message;
        }

        private void UpdateScore(int runs, int wickets, int balls, int target)
        {
            if (scoreText != null)
            {
                scoreText.text = $"{runs}/{wickets}";
            }

            if (equationText != null)
            {
                int needed = Mathf.Max(0, target - runs);
                int remaining = Mathf.Max(0, 6 - balls);
                equationText.text = runs >= target ? "Won" : $"{needed} from {remaining}";
            }
        }

        private void UpdateMessage(string message)
        {
            if (messageText != null)
            {
                messageText.text = message;
            }
        }
    }
}
