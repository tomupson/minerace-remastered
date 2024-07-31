using MineRace.Infrastructure;
using Unity.Netcode;
using VContainer;

namespace MineRace.ConnectionManagement.States
{
    internal abstract class ConnectionState
    {
        [Inject] protected ConnectionManager connectionManager;
        [Inject] protected NetworkManager networkManager;
        [Inject] protected IPublisher<ConnectStatus> connectStatusPublisher;

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
