using System.Collections.Generic;
using System.Linq;
using MineRace.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

[RequireComponent(typeof(NetworkHooks))]
public class ServerGameState : GameStateBehaviour
{
    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly NetworkManager networkManager;

    private Player[] players;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;
    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private NetworkGameState networkGameState;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterInstance(inputReader);
        builder.RegisterInstance(networkGameState);
    }

    protected override void Awake()
    {
        base.Awake();

        NetworkHooks networkHooks = GetComponent<NetworkHooks>();
        networkHooks.OnNetworkSpawnHook += OnNetworkSpawn;
        networkHooks.OnNetworkDespawnHook += OnNetworkDespawn;
    }

    private void OnNetworkSpawn()
    {
        if (!networkManager.IsServer)
        {
            enabled = false;
            return;
        }

        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadEventCompleted;
    }

    private void OnNetworkDespawn()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoadEventCompleted;
    }

    private void OnClientConnected(ulong connectedClientId)
    {
        SpawnPlayer(connectedClientId);
        players = FindObjectsOfType<Player>();

        if (networkManager.ConnectedClientsIds.Count == connectionManager.MaxConnectedPlayers)
        {
            networkGameState.State.Value = GameState.WaitingForPlayersReady;
        }
    }

    private void OnSceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            SpawnPlayer(clientId);
        }
    }

    // For the main player, we start the network manager then transition the scene.
    // Because of this, the player would spawn on the Lobby scene before being moved to the Game scene.
    // This means OnNetworkSpawn runs whilst still on the Lobby, which causes the player to feel the effects of gravity and lose its proper spawn point.
    // Thus, we configure the network manager to not spawn the player, and do it ourselves manually when the SceneManager completes the scene transition for the host.
    // For the clients, they don't exist when the scene transition starts, so aren't listed in the connected clients when the OnLoadEventCompleted event fires.
    // However, they will be automatically transitioned to the Game scene when the connect, therefore we can spawn them then.
    private Player SpawnPlayer(ulong clientId)
    {
        GameObject playerObject = Container.Instantiate(playerPrefab);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, destroyWithScene: true);

        SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(clientId);
        if (sessionPlayerData is { HasCharacterSpawned: true })
        {
            playerObject.transform.position = sessionPlayerData.Value.PlayerPosition;
        }
        else
        {
            playerObject.transform.position = new Vector3(GetSpawnPositionX(clientId), 100, 0);
        }

        Player player = playerObject.GetComponent<Player>();
        player.NetworkPlayerState.State.OnValueChanged += OnPlayerStateChanged;
        return player;
    }

    private float GetSpawnPositionX(ulong clientId)
    {
        const float leftRightPadding = 6;
        float availableWidth = levelData.mapWidth - 2 * leftRightPadding;
        float spacing = availableWidth / connectionManager.MaxConnectedPlayers - 1;
        return leftRightPadding + (clientId + 1) * spacing;
    }

    private void OnPlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (networkGameState.State.Value)
        {
            case GameState.WaitingForPlayersReady:
                CheckForReady(newState);
                break;
            case GameState.InGame:
                CheckForGameOver(newState);
                break;
        }
    }

    private void CheckForReady(PlayerState playerState)
    {
        if (playerState == PlayerState.Ready && !players.Any(p => p.NetworkPlayerState.State.Value != PlayerState.Ready))
        {
            networkGameState.State.Value = GameState.PregameCountdown;
            SessionManager.Instance.OnGameStarted();
        }
    }

    private void CheckForGameOver(PlayerState playerState)
    {
        if (playerState == PlayerState.Completed && !players.Any(p => p.NetworkPlayerState.State.Value < PlayerState.Completed))
        {
            networkGameState.State.Value = GameState.Completed;
            SessionManager.Instance.OnGameEnded();
        }
    }
}
