using System.Collections;
using System.Threading.Tasks;
using MineRace.Infrastructure;
using UnityEngine;
using VContainer;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class ClientReconnectingState : ClientConnectingState
    {
        private const float SecondsBeforeFirstAttempt = 1;
        private const float SecondsBetweenAttempts = 5;

        [Inject] private readonly IPublisher<ReconnectMessage> reconnectMessagePublisher;

        private Coroutine reconnectCoroutine;
        private int numAttempts;

        public override void Enter()
        {
            numAttempts = 0;
            reconnectCoroutine = connectionManager.StartCoroutine(ReconnectCoroutine());
        }

        public override void Exit()
        {
            if (reconnectCoroutine != null)
            {
                connectionManager.StopCoroutine(reconnectCoroutine);
                reconnectCoroutine = null;
            }
            reconnectMessagePublisher.Publish(new ReconnectMessage(connectionManager.NumReconnectAttempts, connectionManager.NumReconnectAttempts));
        }

        public override void OnClientConnected(ulong clientId)
        {
            connectionManager.ChangeState(connectionManager.ClientConnectedState);
        }

        public override void OnClientDisconnect(ulong clientId)
        {
            string disconnectReason = networkManager.DisconnectReason;
            if (numAttempts < connectionManager.NumReconnectAttempts)
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    reconnectCoroutine = connectionManager.StartCoroutine(ReconnectCoroutine());
                }
                else
                {
                    ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    connectStatusPublisher.Publish(connectStatus);
                    switch (connectStatus)
                    {
                        case ConnectStatus.UserRequestedDisconnect:
                        case ConnectStatus.HostEndedSession:
                        case ConnectStatus.ServerFull:
                            connectionManager.ChangeState(connectionManager.OfflineState);
                            break;
                        default:
                            reconnectCoroutine = connectionManager.StartCoroutine(ReconnectCoroutine());
                            break;
                    }
                }
            }
            else
            {
                if (string.IsNullOrEmpty(disconnectReason))
                {
                    connectStatusPublisher.Publish(ConnectStatus.GenericDisconnect);
                }
                else
                {
                    ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                    connectStatusPublisher.Publish(connectStatus);
                }

                connectionManager.ChangeState(connectionManager.OfflineState);
            }
        }

        private IEnumerator ReconnectCoroutine()
        {
            if (numAttempts > 0)
            {
                yield return new WaitForSeconds(SecondsBetweenAttempts);
            }

            Debug.Log("Lost connection to host, trying to reconnect...");

            networkManager.Shutdown();

            yield return new WaitWhile(() => networkManager.ShutdownInProgress);

            Debug.Log($"Reconnecting, attempt {numAttempts + 1}/{connectionManager.NumReconnectAttempts}...");
            reconnectMessagePublisher.Publish(new ReconnectMessage(numAttempts, connectionManager.NumReconnectAttempts));

            // If first attempt, wait some time before attempting to reconnect to give time to services to update
            // (i.e. if in a Lobby and the host shuts down unexpectedly, this will give enough time for the lobby to be
            // properly deleted so that we don't reconnect to an empty lobby
            if (numAttempts == 0)
            {
                yield return new WaitForSeconds(SecondsBeforeFirstAttempt);
            }

            numAttempts++;

            Task<(bool Success, bool ShouldTryAgain)> reconnectingSetupTask = SetupClientReconnectionAsync();
            yield return new WaitUntil(() => reconnectingSetupTask.IsCompleted);

            if (!reconnectingSetupTask.IsFaulted && reconnectingSetupTask.Result.Success)
            {
                Task connectingTask = ConnectClientAsync();
                yield return new WaitUntil(() => connectingTask.IsCompleted);
            }
            else
            {
                if (!reconnectingSetupTask.Result.ShouldTryAgain)
                {
                    numAttempts = connectionManager.NumReconnectAttempts;
                }

                OnClientDisconnect(clientId: 0);
            }
        }

        private async Task<(bool Success, bool ShouldTryAgain)> SetupClientReconnectionAsync()
        {
            if (lobbyManager.ActiveLobby == null)
            {
                Debug.Log("Lobby does not exist anymore, stopping reconnection attempts.");
                return (false, false);
            }

            bool success = await lobbyManager.TryReconnectToLobby();
            Debug.Log(success ? "Successfully reconnected to lobby." : "Failed to reconnect to lobby.");
            return (success, true);
        }
    }
}
