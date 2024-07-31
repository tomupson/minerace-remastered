namespace MineRace.ConnectionManagement
{
    public readonly struct ReconnectMessage
    {
        public int CurrentAttempt { get; }
        public int MaxAttempt { get; }

        public ReconnectMessage(int currentAttempt, int maxAttempt)
        {
            CurrentAttempt = currentAttempt;
            MaxAttempt = maxAttempt;
        }
    }
}
