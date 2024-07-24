using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    private const float LobbyHeartbeatIntervalSeconds = 15f;
    private const string JoinCodeKey = "JOIN_CODE";

    public static LobbyManager Instance { get; private set; }

    public event Action OnLobbyCreating;
    public event Action OnLobbyCreationFailed;
    public event Action OnJoiningLobby;
    public event Action OnLobbyJoinFailed;

    [SerializeField] private int lobbySize = 2;

    private Lobby activeLobby;
    private float lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    private async void Update()
    {
        if (activeLobby != null)
        {
            lobbyHeartbeatTimer -= Time.deltaTime;
            if (lobbyHeartbeatTimer <= 0f)
            {
                lobbyHeartbeatTimer = LobbyHeartbeatIntervalSeconds;
                await Lobbies.Instance.SendHeartbeatPingAsync(activeLobby.Id);
            }
        }
    }

    public async Task<bool> TryCreateLobby(string gameName, string gamePassword)
    {
        OnLobbyCreating?.Invoke();
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobbySize - 1);
            RelayServerData relayData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);
            ConnectionManager.Instance.StartHost();

            CreateLobbyOptions options = new CreateLobbyOptions();
            if (!string.IsNullOrWhiteSpace(gamePassword))
            {
                options.Password = gamePassword;
            }

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            options.Data = new Dictionary<string, DataObject>();
            options.Data.Add(JoinCodeKey, new DataObject(DataObject.VisibilityOptions.Member, joinCode));

            activeLobby = await Lobbies.Instance.CreateLobbyAsync(gameName, lobbySize, options);
            return true;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
        catch (RelayServiceException ex)
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
            lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            if (!lobby.Data.TryGetValue(JoinCodeKey, out DataObject lobbyData))
            {
                return false;
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyData.Value);

            RelayServerData relayData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            ConnectionManager.Instance.StartClient();
            return true;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
        catch (RelayServiceException ex)
        {
            Debug.LogException(ex);
        }
        OnLobbyJoinFailed?.Invoke();
        return false;
    }

    public async Task DeleteLobby()
    {
        if (activeLobby == null)
        {
            return;
        }

        try
        {
            await LobbyService.Instance.DeleteLobbyAsync(activeLobby.Id);
            activeLobby = null;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
        }
    }
}
