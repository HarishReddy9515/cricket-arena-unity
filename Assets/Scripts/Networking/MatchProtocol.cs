using System;

namespace CricketArena.Networking
{
    [Serializable]
    public sealed class MatchMessage
    {
        public string type;
        public string roomCode;
        public string playerId;
        public bool ready;
        public float timing;
        public string intent;
        public long clientTime;
    }

    [Serializable]
    public sealed class DeliveryMessage
    {
        public string type;
        public DeliveryDto delivery;
        public RoomDto room;
    }

    [Serializable]
    public sealed class DeliveryDto
    {
        public string name;
        public float speed;
        public float swing;
        public float bounce;
        public float difficulty;
    }

    [Serializable]
    public sealed class RoomDto
    {
        public string code;
        public string status;
        public int target;
        public int score;
        public int wickets;
        public int balls;
    }
}
