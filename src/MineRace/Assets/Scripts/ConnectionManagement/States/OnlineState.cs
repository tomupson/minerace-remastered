namespace MineRace.ConnectionManagement.States
{
    internal abstract class OnlineState : ConnectionState
    {
        public override void OnUserRequestedShutdown()
        {
            connectStatusPublisher.Publish(ConnectStatus.UserRequestedDisconnect);
            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        public override void OnTransportFailure()
        {
            connectionManager.ChangeState(connectionManager.OfflineState);
        }
    }
}
