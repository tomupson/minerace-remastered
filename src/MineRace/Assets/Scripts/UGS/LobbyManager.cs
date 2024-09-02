using System.Collections.Generic;
using System.Threading.Tasks;
using MineRace.Infrastructure;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace MineRace.UGS
{
    public sealed class LobbyManager : ITickable
    {
        private const float LobbyHeartbeatIntervalSeconds = 15f;

        [Inject] private readonly IPublisher<LobbyStatus> lobbyStatusPublisher;
        private readonly RateLimitCooldown queryRateLimit = new RateLimitCooldown(1f);

        private float lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;
        private bool isTracking;
        private ILobbyEvents lobbyEvents;
        private LobbyEventConnectionState lobbyEventConnectionState;

        public TrackedLobby ActiveLobby { get; private set; }

        public async void Tick()
        {
            if (isTracking && ActiveLobby.IsLocalPlayerHost())
            {
                lobbyHeartbeatTimer -= Time.deltaTime;
                if (lobbyHeartbeatTimer <= 0f)
                {
                    lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;
                    try
                    {
                        await LobbyService.Instance.SendHeartbeatPingAsync(ActiveLobby.LobbyId);
                    }
                    catch (LobbyServiceException ex)
                    {
                        if (ex.Reason != LobbyExceptionReason.LobbyNotFound)
                        {
                            Debug.LogException(ex);
                        }
                    }
                }
            }
        }

        public async Task<List<Lobby>> QueryLobbies()
        {
            if (!queryRateLimit.CanCall)
            {
                return new List<Lobby>();
            }

            try
            {
                QueryLobbiesOptions queryOptions = new QueryLobbiesOptions { Count = 16 };
                queryOptions.Filters = new List<QueryFilter>();
                queryOptions.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT));

                QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);
                return response.Results;
            }
            catch (LobbyServiceException ex)
            {
                if (ex.Reason == LobbyExceptionReason.RateLimited)
                {
                    queryRateLimit.PutOnCooldown();
                }
                else
                {
                    Debug.LogException(ex);
                }
            }

            return null;
        }

        public async Task<bool> TryCreateLobby(string lobbyName, string lobbyPassword, int lobbySize)
        {
            lobbyStatusPublisher.Publish(LobbyStatus.Creating);

            try
            {
                CreateLobbyOptions options = new CreateLobbyOptions();
                if (!string.IsNullOrWhiteSpace(lobbyPassword))
                {
                    options.Password = lobbyPassword;
                }

                Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbySize, options);
                ActiveLobby = new TrackedLobby(lobby);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }

            lobbyStatusPublisher.Publish(LobbyStatus.CreationFailed);
            return false;
        }

        public async Task<bool> TryJoinLobby(Lobby lobby, JoinLobbyByIdOptions options = null)
        {
            lobbyStatusPublisher.Publish(LobbyStatus.Joining);

            try
            {
                lobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
                ActiveLobby = new TrackedLobby(lobby);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }

            lobbyStatusPublisher.Publish(LobbyStatus.JoinFailed);
            return false;
        }

        public async Task<bool> UpdateActiveLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to update lobby when not tracking a lobby");
                return false;
            }

            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.LobbyId, new UpdateLobbyOptions { Data = ActiveLobby.Data });
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }

            return false;
        }

        public async Task<bool> TryReconnectToLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to reconnect to a lobby when not tracking a lobby");
                return false;
            }

            try
            {
                Lobby lobby = await LobbyService.Instance.ReconnectToLobbyAsync(ActiveLobby.LobbyId);
                ActiveLobby = new TrackedLobby(lobby);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                if (ex.Reason != LobbyExceptionReason.LobbyNotFound)
                {
                    Debug.LogException(ex);
                }
            }

            return false;
        }

        public async Task DeleteLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to delete a lobby when not tracking a lobby");
                return;
            }

            if (!ActiveLobby.IsLocalPlayerHost())
            {
                Debug.LogError("Only the host can delete a lobby");
                return;
            }

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(ActiveLobby.LobbyId);
            }
            catch (LobbyServiceException ex)
            {
                if (ex.Reason != LobbyExceptionReason.LobbyNotFound)
                {
                    Debug.LogException(ex);
                }
            }
            finally
            {
                ActiveLobby = null;
            }
        }

        public async Task LeaveLobby()
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to leave a lobby when not tracking a lobby");
                return;
            }

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.LobbyId, AuthenticationService.Instance.PlayerId);
            }
            catch (LobbyServiceException ex)
            {
                if (ex.Reason != LobbyExceptionReason.LobbyNotFound)
                {
                    Debug.LogException(ex);
                }
            }
            finally
            {
                ActiveLobby = null;
            }
        }

        public async Task UpdatePlayerRelayInfo(string allocationId, string connectionInfo)
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to update player relay info on a lobby when not tracking a lobby");
                return;
            }

            try
            {
                await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.LobbyId, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
                {
                    AllocationId = allocationId,
                    ConnectionInfo = connectionInfo
                });
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }
        }

        public async void RemovePlayerFromLobbyAsync(string playerId)
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to remove player from a lobby when not tracking a lobby");
                return;
            }

            if (!ActiveLobby.IsLocalPlayerHost())
            {
                Debug.LogError("Only the host can remove other players from the lobby");
                return;
            }

            try
            {
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.LobbyId, playerId);
            }
            catch (LobbyServiceException ex)
            {
                Debug.Log(ex.Reason);
                if (ex.Reason != LobbyExceptionReason.PlayerNotFound)
                {
                    Debug.LogException(ex);
                }
            }
        }

        public async void BeginTracking()
        {
            if (!isTracking)
            {
                isTracking = true;

                LobbyEventCallbacks lobbyEventCallbacks = new LobbyEventCallbacks();
                lobbyEventCallbacks.LobbyChanged += OnLobbyChanged;
                lobbyEventCallbacks.LobbyEventConnectionStateChanged += OnLobbyEventConnectionStateChanged;
                lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(ActiveLobby.LobbyId, lobbyEventCallbacks);
            }
        }

        private void OnLobbyChanged(ILobbyChanges changes)
        {
            if (changes.LobbyDeleted)
            {
                ActiveLobby = null;
                EndTracking();
                return;
            }

            ActiveLobby.ApplyChanges(changes);
        }

        private void OnLobbyEventConnectionStateChanged(LobbyEventConnectionState state)
        {
            lobbyEventConnectionState = state;
        }

        public async void EndTracking()
        {
            if (isTracking)
            {
                isTracking = false;
                if (lobbyEvents != null && lobbyEventConnectionState != LobbyEventConnectionState.Unsubscribed)
                {
#if UNITY_EDITOR
                    try
                    {
                        await lobbyEvents.UnsubscribeAsync();
                    }
                    catch (LobbyServiceException ex) when (ex.Reason == LobbyExceptionReason.SubscriptionToLobbyLostWhileBusy) { }
#else
                    await lobbyEvents.UnsubscribeAsync();
#endif
                }
            }

            if (ActiveLobby == null)
            {
                return;
            }

            if (ActiveLobby.IsLocalPlayerHost())
            {
                await DeleteLobby();
            }
            else
            {
                await LeaveLobby();
            }
        }
    }
}
