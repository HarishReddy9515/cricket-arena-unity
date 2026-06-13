using System.Collections.Generic;
using CricketArena.Presentation;
using UnityEngine;

namespace CricketArena.Core
{
    public sealed class ReplayRecorder : MonoBehaviour
    {
        [SerializeField] private CameraDirector cameraDirector;
        [SerializeField] private Transform ball;
        [SerializeField] private int maxEvents = 24;

        private readonly List<ReplayEvent> events = new List<ReplayEvent>();

        public IReadOnlyList<ReplayEvent> Events => events;

        public void Record(ShotOutcome outcome)
        {
            if (events.Count >= maxEvents)
            {
                events.RemoveAt(0);
            }

            Vector3 ballPosition = ball != null ? ball.position : Vector3.zero;
            events.Add(new ReplayEvent(Time.time, outcome, ballPosition));
        }

        public void PlayLastHighlight()
        {
            if (events.Count == 0) return;
            cameraDirector?.ShowReplayCamera();
            Invoke(nameof(ReturnToBattingCamera), 2.2f);
        }

        private void ReturnToBattingCamera()
        {
            cameraDirector?.ShowBattingCamera();
        }
    }
}
