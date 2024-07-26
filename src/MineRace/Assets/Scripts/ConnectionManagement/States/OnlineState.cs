using MineRace.Infrastructure;

namespace MineRace.ConnectionManagement.States
{
    internal abstract class OnlineState : ConnectionState
    {
        protected OnlineState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher)
            : base(connectionManager, connectStatusPublisher)
        {
        }

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
