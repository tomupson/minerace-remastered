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

        public Lobby ActiveLobby { get; private set; }

        public async void Tick()
        {
            if (ActiveLobby != null && ActiveLobby.HostId == AuthenticationService.Instance.PlayerId)
            {
                lobbyHeartbeatTimer -= Time.deltaTime;
                if (lobbyHeartbeatTimer <= 0f)
                {
                    lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;
                    await LobbyService.Instance.SendHeartbeatPingAsync(ActiveLobby.Id);
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
                Debug.LogException(ex);
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

                ActiveLobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, lobbySize, options);
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
                ActiveLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }
            lobbyStatusPublisher.Publish(LobbyStatus.JoinFailed);
            return false;
        }

        public async Task<bool> UpdateLobbyData(Dictionary<string, DataObject> data)
        {
            if (ActiveLobby == null)
            {
                Debug.LogWarning("Tried to update lobby when not tracking a lobby");
                return false;
            }

            Dictionary<string, DataObject> currentData = ActiveLobby.Data ?? new Dictionary<string, DataObject>();
            foreach (KeyValuePair<string, DataObject> newData in data)
            {
                if (currentData.ContainsKey(newData.Key))
                {
                    currentData[newData.Key] = newData.Value;
                }
                else
                {
                    currentData.Add(newData.Key, newData.Value);
                }
            }

            try
            {
                await LobbyService.Instance.UpdateLobbyAsync(ActiveLobby.Id, new UpdateLobbyOptions { Data = currentData });
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
                ActiveLobby = await LobbyService.Instance.ReconnectToLobbyAsync(ActiveLobby.Id);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
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

            if (ActiveLobby.HostId != AuthenticationService.Instance.PlayerId)
            {
                Debug.LogError("Only the host can delete a lobby");
                return;
            }

            try
            {
                await LobbyService.Instance.DeleteLobbyAsync(ActiveLobby.Id);
                ActiveLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
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
                await LobbyService.Instance.RemovePlayerAsync(ActiveLobby.Id, AuthenticationService.Instance.PlayerId);
                ActiveLobby = null;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
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
                await LobbyService.Instance.UpdatePlayerAsync(ActiveLobby.Id, AuthenticationService.Instance.PlayerId, new UpdatePlayerOptions
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
    }
}
