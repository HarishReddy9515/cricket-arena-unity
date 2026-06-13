using CricketArena.Gameplay;
using UnityEngine;

namespace CricketArena.UI
{
    public sealed class MobileControlsController : MonoBehaviour
    {
        [SerializeField] private BattingController battingController;
        [SerializeField] private BowlingController bowlingController;

        public void HitStraight()
        {
            battingController?.SetIntent((int)ShotIntent.Straight);
            battingController?.PlayShot();
        }

        public void LoftLeft()
        {
            battingController?.SetIntent((int)ShotIntent.LoftLeft);
            battingController?.PlayShot();
        }

        public void CutRight()
        {
            battingController?.SetIntent((int)ShotIntent.CutRight);
            battingController?.PlayShot();
        }

        public void Bowl()
        {
            bowlingController?.BowlNext();
        }
    }
}
