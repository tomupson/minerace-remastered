public readonly struct PauseStateChangedMessage
{
    public bool IsPaused { get; }

    public PauseStateChangedMessage(bool isPaused)
    {
        IsPaused = isPaused;
    }
}
