using System.Collections.Generic;
using MineRace.Infrastructure;
using MineRace.UGS;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class StartingHostState : OnlineState
    {
        private readonly LobbyManager lobbyManager;

        public StartingHostState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher, LobbyManager lobbyManager)
            : base(connectionManager, connectStatusPublisher)
        {
            this.lobbyManager = lobbyManager;
        }

        public override void Enter()
        {
            StartHostInternal();
        }

        public override void Exit() { }

        public override void OnServerStarted()
        {
            connectStatusPublisher.Publish(ConnectStatus.Success);
            connectionManager.ChangeState(connectionManager.HostingState);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            ulong clientId = request.ClientNetworkId;
            if (clientId == NetworkManager.Singleton.LocalClientId)
            {
                response.Approved = true;
            }
        }

        public override void OnServerStopped()
        {
            StartHostFailed();
        }

        private async void StartHostInternal()
        {
            try
            {
                Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(connectionManager.MaxConnectedPlayers);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

                Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
                lobbyData.Add("RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode));

                await lobbyManager.UpdateLobbyData(lobbyData);
                await lobbyManager.UpdatePlayerRelayInfo(hostAllocation.AllocationIdBytes.ToString(), joinCode);

                UnityTransport utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
                utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));

                if (!NetworkManager.Singleton.StartHost())
                {
                    StartHostFailed();
                }
            }
            catch
            {
                StartHostFailed();
                throw;
            }
        }

        private void StartHostFailed()
        {
            connectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            connectionManager.ChangeState(connectionManager.OfflineState);
        }
    }
}
