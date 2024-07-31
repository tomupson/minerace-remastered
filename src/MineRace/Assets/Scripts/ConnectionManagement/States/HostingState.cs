using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class HostingState : OnlineState
    {
        public override void Enter()
        {
            networkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }

        public override void Exit() { }

        public override void OnUserRequestedShutdown()
        {
            string reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (int i = networkManager.ConnectedClientsIds.Count - 1; i >= 0; i--)
            {
                ulong clientId = networkManager.ConnectedClientsIds[i];
                if (clientId != networkManager.LocalClientId)
                {
                    networkManager.DisconnectClient(clientId, reason);
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
            if (networkManager.ConnectedClientsIds.Count >= connectionManager.MaxConnectedPlayers)
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(ConnectStatus.ServerFull);
                return;
            }

            response.Approved = true;
        }
    }
}
