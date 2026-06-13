namespace CricketArena.Core
{
    public readonly struct ShotOutcome
    {
        public readonly int Runs;
        public readonly bool IsWicket;
        public readonly string Message;
        public readonly float Power;

        public ShotOutcome(int runs, bool isWicket, string message, float power)
        {
            Runs = runs;
            IsWicket = isWicket;
            Message = message;
            Power = power;
        }
    }
}
