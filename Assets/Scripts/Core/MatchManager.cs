using UnityEngine;
using UnityEngine.Events;

namespace CricketArena.Core
{
    public enum MatchPhase
    {
        Menu,
        WaitingForDelivery,
        DeliveryLive,
        ShotResolved,
        InningsComplete
    }

    public sealed class MatchManager : MonoBehaviour
    {
        [Header("Match")]
        [SerializeField] private int targetRuns = 24;
        [SerializeField] private int maxBalls = 6;
        [SerializeField] private int maxWickets = 2;

        [Header("Events")]
        public UnityEvent<int, int, int, int> OnScoreChanged;
        public UnityEvent<string> OnMessage;

        public MatchPhase Phase { get; private set; } = MatchPhase.Menu;
        public CricketGameMode Mode { get; private set; } = CricketGameMode.QuickMatch;
        public int Runs { get; private set; }
        public int Wickets { get; private set; }
        public int Balls { get; private set; }
        public int TargetRuns => targetRuns;
        public int MaxBalls => maxBalls;
        public int MaxWickets => maxWickets;

        public void Configure(GameModeConfig config)
        {
            Mode = config.Mode;
            targetRuns = Mathf.Max(1, config.TargetRuns);
            maxBalls = Mathf.Max(1, config.MaxBalls);
            maxWickets = Mathf.Max(1, config.MaxWickets);
            Publish(config.DisplayName);
        }

        public void StartChase()
        {
            Runs = 0;
            Wickets = 0;
            Balls = 0;
            Phase = MatchPhase.WaitingForDelivery;
            Publish("Chase started");
        }

        public void MarkDeliveryLive()
        {
            if (Phase == MatchPhase.InningsComplete) return;
            Phase = MatchPhase.DeliveryLive;
            Publish("Ball released");
        }

        public void ApplyOutcome(ShotOutcome outcome)
        {
            if (Phase != MatchPhase.DeliveryLive) return;

            Runs += outcome.Runs;
            Wickets += outcome.IsWicket ? 1 : 0;
            Balls += 1;
            Phase = MatchPhase.ShotResolved;
            Publish(outcome.Message);

            if (Runs >= targetRuns || Balls >= maxBalls || Wickets >= maxWickets)
            {
                Phase = MatchPhase.InningsComplete;
                Publish(Runs >= targetRuns ? "You win" : "Match lost");
            }
            else
            {
                Phase = MatchPhase.WaitingForDelivery;
            }
        }

        public void SyncAuthoritativeState(int runs, int wickets, int balls, string status, string message)
        {
            Runs = Mathf.Max(0, runs);
            Wickets = Mathf.Max(0, wickets);
            Balls = Mathf.Max(0, balls);

            Phase = status switch
            {
                "live" => MatchPhase.WaitingForDelivery,
                "finished" => MatchPhase.InningsComplete,
                "waiting" => MatchPhase.Menu,
                _ => Phase
            };

            Publish(string.IsNullOrWhiteSpace(message) ? "Server state synced" : message);
        }

        private void Publish(string message)
        {
            OnScoreChanged?.Invoke(Runs, Wickets, Balls, targetRuns);
            OnMessage?.Invoke(message);
        }
    }
}
