using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class HostingState : OnlineState
    {
        public HostingState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher)
            : base(connectionManager, connectStatusPublisher)
        {
        }

        public override void Enter()
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        public override void Exit() { }

        public override void OnUserRequestedShutdown()
        {
            string reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (int i = NetworkManager.Singleton.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                ulong clientId = NetworkManager.Singleton.ConnectedClientsIds[i];
                if (clientId != NetworkManager.Singleton.LocalClientId)
                {
                    NetworkManager.Singleton.DisconnectClient(clientId, reason);
                }
            }

            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        public override void OnServerStopped()
        {
            connectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            if (NetworkManager.Singleton.ConnectedClientsIds.Count >= connectionManager.MaxConnectedPlayers)
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(ConnectStatus.ServerFull);
                return;
            }

            response.Approved = true;
        }
    }
}
