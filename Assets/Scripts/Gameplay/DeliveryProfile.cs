using UnityEngine;

namespace CricketArena.Gameplay
{
    [System.Serializable]
    public struct DeliveryProfile
    {
        public string Name;
        public float Speed;
        public float Swing;
        public float Bounce;
        public float Difficulty;

        public static DeliveryProfile Default => new DeliveryProfile
        {
            Name = "Fast length",
            Speed = 26f,
            Swing = 0.05f,
            Bounce = 0.65f,
            Difficulty = 0.6f
        };
    }
}
