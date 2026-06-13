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

        public static MatchMessage JoinRoom(string roomCode)
        {
            return new MatchMessage { type = MatchEvents.JoinRoom, roomCode = roomCode };
        }

        public static MatchMessage Ready(bool value)
        {
            return new MatchMessage { type = MatchEvents.Ready, ready = value };
        }

        public static MatchMessage RequestDelivery()
        {
            return new MatchMessage { type = MatchEvents.RequestDelivery };
        }

        public static MatchMessage Shot(float timing, string intent)
        {
            return new MatchMessage { type = MatchEvents.Shot, timing = timing, intent = intent };
        }

        public static MatchMessage Ping(long clientTime)
        {
            return new MatchMessage { type = MatchEvents.Ping, clientTime = clientTime };
        }
    }

    [Serializable]
    public sealed class DeliveryMessage
    {
        public string type;
        public DeliveryDto delivery;
        public RoomDto room;
        public OutcomeDto outcome;
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

    [Serializable]
    public sealed class OutcomeDto
    {
        public int runs;
        public bool wicket;
        public string message;
        public float quality;
    }

    public static class MatchEvents
    {
        public const string Connected = "connected";
        public const string JoinRoom = "join_room";
        public const string Ready = "ready";
        public const string RequestDelivery = "request_delivery";
        public const string Delivery = "delivery";
        public const string Shot = "shot";
        public const string MatchState = "match_state";
        public const string RoomState = "room_state";
        public const string Ping = "ping";
        public const string Pong = "pong";
    }
}
