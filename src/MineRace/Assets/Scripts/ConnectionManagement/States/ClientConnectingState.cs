using System;
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace MineRace.ConnectionManagement.States
{
    internal class ClientConnectingState : OnlineState
    {
        private string playerName;

        public ClientConnectingState Configure(string playerName)
        {
            this.playerName = playerName;
            return this;
        }

        public override async void Enter()
        {
            await ConnectClientAsync();
        }

        public override void Exit() { }

        public override void OnClientConnected(ulong clientId)
        {
            connectStatusPublisher.Publish(ConnectStatus.Success);
            connectionManager.ChangeState(connectionManager.ClientConnectedState);
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            StartingClientFailed();
        }

        private void StartingClientFailed()
        {
            string disconnectReason = networkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                connectStatusPublisher.Publish(ConnectStatus.StartClientFailed);
            }
            else
            {
                ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                connectStatusPublisher.Publish(connectStatus);
            }

            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        protected async Task ConnectClientAsync()
        {
            try
            {
                if (lobbyManager.ActiveLobby == null)
                {
                    throw new Exception("Trying to start relay while Lobby isn't set");
                }

                SetConnectionPayload(playerName);

                string relayJoinCode = lobbyManager.ActiveLobby.Data.TryGetValue("RelayJoinCode", out DataObject joinCodeData) ? joinCodeData.Value : null;

                JoinAllocation joinedAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                await lobbyManager.UpdatePlayerRelayInfo(joinedAllocation.AllocationId.ToString(), relayJoinCode);

                UnityTransport utp = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;
                utp.SetRelayServerData(new RelayServerData(joinedAllocation, "dtls"));

                if (!networkManager.StartClient())
                {
                    throw new Exception("NetworkManager StartClient failed");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Error connecting client, see following exception");
                Debug.LogException(ex);
                StartingClientFailed();
                throw;
            }
        }
    }
}
