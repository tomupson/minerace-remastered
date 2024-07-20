using System.Collections.Generic;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using System.Threading.Tasks;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance { get; private set; }

    [SerializeField] private int lobbySize = 2;

    private Lobby activeLobby;
    private float lobbyHeartbeatTimer;

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
                await Lobbies.Instance.SendHeartbeatPingAsync(activeLobby.Id);
            }
        }
    }

    public async Task<bool> TryCreateLobby(string gameName, string gamePassword)
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(lobbySize);
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
            options.Data.Add("JOIN_CODE", new DataObject(DataObject.VisibilityOptions.Member, joinCode));

            activeLobby = await Lobbies.Instance.CreateLobbyAsync(gameName, lobbySize, options);
            return true;
        }
        catch (LobbyServiceException) { }
        catch (RelayServiceException) { }
        return false;
    }

    public async Task<bool> TryJoinLobby(Lobby lobby, JoinLobbyByIdOptions options = null)
    {
        try
        {
            lobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id, options);
            if (!lobby.Data.TryGetValue("JOIN_CODE", out DataObject lobbyData))
            {
                return false;
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyData.Value);

            RelayServerData relayData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            ConnectionManager.Instance.StartClient();
            return true;
        }
        catch (LobbyServiceException) { }
        catch (RelayServiceException) { }
        return false;
    }
}
