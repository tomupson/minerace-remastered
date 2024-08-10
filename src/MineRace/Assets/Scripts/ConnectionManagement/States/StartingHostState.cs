using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class StartingHostState : OnlineState
    {
        private string playerName;

        public StartingHostState Configure(string playerName)
        {
            this.playerName = playerName;
            return this;
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

            if (clientId == networkManager.LocalClientId)
            {
                ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(Encoding.UTF8.GetString(request.Payload));

                SessionManager.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.PlayerId,
                    new SessionPlayerData(clientId, connectionPayload.PlayerName, isConnected: true));

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
                SetConnectionPayload(playerName);

                Allocation hostAllocation = await RelayService.Instance.CreateAllocationAsync(connectionManager.MaxConnectedPlayers);
                string joinCode = await RelayService.Instance.GetJoinCodeAsync(hostAllocation.AllocationId);

                Dictionary<string, DataObject> lobbyData = new Dictionary<string, DataObject>();
                lobbyData.Add("RelayJoinCode", new DataObject(DataObject.VisibilityOptions.Member, joinCode));

                await lobbyManager.UpdateLobbyData(lobbyData);
                await lobbyManager.UpdatePlayerRelayInfo(hostAllocation.AllocationIdBytes.ToString(), joinCode);

                UnityTransport utp = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;
                utp.SetRelayServerData(new RelayServerData(hostAllocation, "dtls"));

                if (!networkManager.StartHost())
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
