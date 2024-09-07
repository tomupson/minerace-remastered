using System.Text;
using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class HostingState : OnlineState
    {
        [Inject] private readonly IPublisher<NetworkConnectionEventMessage> connectionEventPublisher;

        public override void Enter()
        {
            networkManager.SceneManager.LoadScene("Game", LoadSceneMode.Single);

            if (lobbyManager.ActiveLobby != null)
            {
                lobbyManager.BeginTracking();
            }
        }

        public override void Exit()
        {
            SessionManager.Instance.OnServerEnded();
        }

        public override void OnClientConnected(ulong clientId)
        {
            SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(clientId);
            if (sessionPlayerData.HasValue)
            {
                connectionEventPublisher.Publish(new NetworkConnectionEventMessage(clientId, ConnectStatus.Success, sessionPlayerData.Value.PlayerName));
                return;
            }

            // This should not happen since player data is assigned during connection approval
            Debug.LogError($"No player data associated with client {clientId}");
            string reason = JsonUtility.ToJson(ConnectStatus.GenericDisconnect);
            networkManager.DisconnectClient(clientId, reason);
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId == networkManager.LocalClientId)
            {
                return;
            }

            string playerId = SessionManager.Instance.GetPlayerId(clientId);
            if (playerId != null)
            {
                SessionPlayerData? sessionData = SessionManager.Instance.GetPlayerData(playerId);
                if (sessionData.HasValue)
                {
                    connectionEventPublisher.Publish(new NetworkConnectionEventMessage(clientId, ConnectStatus.GenericDisconnect, sessionData.Value.PlayerName));
                }

                SessionManager.Instance.DisconnectClient(clientId);
            }
        }

        public override void OnUserRequestedShutdown()
        {
            string reason = JsonUtility.ToJson(ConnectStatus.HostEndedSession);
            for (int clientIdIdx = networkManager.ConnectedClientsIds.Count - 1; clientIdIdx >= 0; clientIdIdx--)
            {
                ulong clientId = networkManager.ConnectedClientsIds[clientIdIdx];
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

        public override async void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            ulong clientId = request.ClientNetworkId;
            ConnectionPayload connectionPayload = JsonUtility.FromJson<ConnectionPayload>(Encoding.UTF8.GetString(request.Payload));

            if (networkManager.ConnectedClientsIds.Count >= connectionManager.MaxConnectedPlayers)
            {
                response.Approved = false;
                response.Reason = JsonUtility.ToJson(ConnectStatus.ServerFull);
                if (lobbyManager.ActiveLobby != null)
                {
                    await lobbyManager.RemovePlayerFromLobby(connectionPayload.PlayerId);
                }
                return;
            }

            SessionManager.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.PlayerId,
                new SessionPlayerData(clientId, connectionPayload.PlayerName, isConnected: true));

            response.Approved = true;
        }
    }
}
