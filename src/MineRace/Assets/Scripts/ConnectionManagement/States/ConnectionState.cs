using MineRace.Infrastructure;
using MineRace.UGS;
using Unity.Netcode;
using VContainer;

namespace MineRace.ConnectionManagement.States
{
    internal abstract class ConnectionState
    {
        [Inject] protected readonly ConnectionManager connectionManager;
        [Inject] protected readonly NetworkManager networkManager;
        [Inject] protected readonly LobbyManager lobbyManager;
        [Inject] protected readonly IPublisher<ConnectStatus> connectStatusPublisher;

        public abstract void Enter();

        public abstract void Exit();

        public virtual void OnClientConnected(ulong clientId) { }

        public virtual void OnClientDisconnect(ulong clientId) { }

        public virtual void OnServerStarted() { }

        public virtual void StartClient(string playerName) { }

        public virtual void StartHost(string playerName) { }

        public virtual void OnUserRequestedShutdown() { }

        public virtual void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) { }

        public virtual void OnTransportFailure() { }

        public virtual void OnServerStopped() { }
    }
}
