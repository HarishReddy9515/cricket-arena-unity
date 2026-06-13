using UnityEngine;

namespace CricketArena.Core
{
    [System.Serializable]
    public readonly struct ReplayEvent
    {
        public readonly float Time;
        public readonly int Runs;
        public readonly bool IsWicket;
        public readonly string Message;
        public readonly Vector3 BallPosition;

        public ReplayEvent(float time, ShotOutcome outcome, Vector3 ballPosition)
        {
            Time = time;
            Runs = outcome.Runs;
            IsWicket = outcome.IsWicket;
            Message = outcome.Message;
            BallPosition = ballPosition;
        }
    }
}
