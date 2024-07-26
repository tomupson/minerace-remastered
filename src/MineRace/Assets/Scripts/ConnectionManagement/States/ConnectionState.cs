using MineRace.Infrastructure;
using Unity.Netcode;

namespace MineRace.ConnectionManagement.States
{
    internal abstract class ConnectionState
    {
        protected ConnectionManager connectionManager;
        protected IPublisher<ConnectStatus> connectStatusPublisher;

        protected ConnectionState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher)
        {
            this.connectionManager = connectionManager;
            this.connectStatusPublisher = connectStatusPublisher;
        }

        public abstract void Enter();

        public abstract void Exit();

        public virtual void OnClientConnected(ulong clientId) { }

        public virtual void OnClientDisconnect(ulong clientId) { }

        public virtual void OnServerStarted() { }

        public virtual void StartClient() { }

        public virtual void StartHost() { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }

        public virtual void OnServerStopped() { }
    }
}
