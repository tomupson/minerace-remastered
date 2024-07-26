namespace MineRace.ConnectionManagement
{
    public enum ConnectStatus
    {
        Unknown,
        Success,
        ServerFull,
        UserRequestedDisconnect,
        GenericDisconnect,
        Reconnecting,
        HostEndedSession,
        StartHostFailed,
        StartClientFailed
    }
}
