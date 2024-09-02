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
                SetConnectionPayload(playerName);

                string relayJoinCode = lobbyManager.ActiveLobby.RelayJoinCode;

                JoinAllocation joinedAllocation = await RelayService.Instance.JoinAllocationAsync(relayJoinCode);
                await lobbyManager.UpdatePlayerRelayInfo(joinedAllocation.AllocationId.ToString(), relayJoinCode);

                UnityTransport utp = (UnityTransport)networkManager.NetworkConfig.NetworkTransport;
                utp.SetRelayServerData(new RelayServerData(joinedAllocation, "dtls"));

                if (!networkManager.StartClient())
                {
                    StartingClientFailed();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                StartingClientFailed();
                throw;
            }
        }
    }
}
