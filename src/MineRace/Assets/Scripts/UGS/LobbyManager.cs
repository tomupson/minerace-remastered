using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace MineRace.UGS
{
    public class LobbyManager : MonoBehaviour
    {
        private const float LobbyHeartbeatIntervalSeconds = 15f;

        public static LobbyManager Instance { get; private set; }

        public event Action OnLobbyCreating;
        public event Action OnLobbyCreationFailed;
        public event Action OnJoiningLobby;
        public event Action OnLobbyJoinFailed;

        [SerializeField] private int lobbySize = 2;

        private float lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;

        public Lobby ActiveLobby { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        private async void Update()
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

        public async Task<bool> TryCreateLobby(string gameName, string gamePassword)
        {
            OnLobbyCreating?.Invoke();
            try
            {
                CreateLobbyOptions options = new CreateLobbyOptions();
                if (!string.IsNullOrWhiteSpace(gamePassword))
                {
                    options.Password = gamePassword;
                }

                ActiveLobby = await LobbyService.Instance.CreateLobbyAsync(gameName, lobbySize, options);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }
            OnLobbyCreationFailed?.Invoke();
            return false;
        }

        public async Task<bool> TryJoinLobby(Lobby lobby, JoinLobbyByIdOptions options = null)
        {
            OnJoiningLobby?.Invoke();
            try
            {
                ActiveLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, options);
                return true;
            }
            catch (LobbyServiceException ex)
            {
                Debug.LogException(ex);
            }
            OnLobbyJoinFailed?.Invoke();
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
