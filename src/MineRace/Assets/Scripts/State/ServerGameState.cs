using System.Collections.Generic;
using System.Linq;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
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

    private DisposableGroup subscriptions;
    private NetworkHooks networkHooks;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;
    [SerializeField] private NetworkGameState networkGameState;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterComponent(new NetworkedMessageChannel<NetworkChatMessage>()).AsImplementedInterfaces();
        builder.RegisterInstance(networkGameState);
    }

    protected override void Awake()
    {
        base.Awake();

        networkHooks = GetComponent<NetworkHooks>();
        networkHooks.OnNetworkSpawnHook += OnNetworkSpawn;
        networkHooks.OnNetworkDespawnHook += OnNetworkDespawn;
    }

    protected override void OnDestroy()
    {
        networkHooks.OnNetworkSpawnHook -= OnNetworkSpawn;
        networkHooks.OnNetworkDespawnHook -= OnNetworkDespawn;

        subscriptions?.Dispose();
    }

    private void OnNetworkSpawn()
    {
        if (!networkManager.IsServer)
        {
            enabled = false;
            return;
        }

        networkManager.OnClientConnectedCallback += OnClientConnected;
        networkManager.OnClientDisconnectCallback += OnClientDisconnected;
        networkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadEventCompleted;
    }

    private void OnNetworkDespawn()
    {
        networkManager.OnClientConnectedCallback -= OnClientConnected;
        networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        networkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoadEventCompleted;
    }

    private void OnClientConnected(ulong clientId)
    {
        SpawnPlayer(clientId);
        CheckAllPlayersConnected();
    }

    private void OnClientDisconnected(ulong clientId)
    {
        if (networkManager.ShutdownInProgress)
        {
            return;
        }

        for (int playerIdx = 0; playerIdx < networkGameState.Players.Count; playerIdx++)
        {
            if (networkGameState.Players[playerIdx].clientId == clientId)
            {
                networkGameState.Players.RemoveAt(playerIdx);
                break;
            }
        }

        if (networkManager.ConnectedClientsIds.Count < connectionManager.MaxConnectedPlayers && networkGameState.State.Value <= GameState.WaitingForPlayersReady)
        {
            networkGameState.State.Value = GameState.WaitingForPlayers;
            foreach (Player player in Player.GetSpawnedPlayers())
            {
                player.NetworkPlayerState.State.Value = PlayerState.WaitingForPlayers;
            }
        }
    }

    private void OnSceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in clientsCompleted)
        {
            SpawnPlayer(clientId);
            CheckAllPlayersConnected();
        }
    }

    // For the main player, we start the network manager then transition the scene.
    // Because of this, the player would spawn on the Lobby scene before being moved to the Game scene.
    // This means OnNetworkSpawn runs whilst still on the Lobby, which causes the player to feel the effects of gravity and lose its proper spawn point.
    // Thus, we configure the network manager to not spawn the player, and do it ourselves manually when the SceneManager completes the scene transition for the host.
    // For the clients, they don't exist when the scene transition starts, so aren't listed in the connected clients when the OnLoadEventCompleted event fires.
    // However, they will be automatically transitioned to the Game scene when the connect, therefore we can spawn them then.
    private void SpawnPlayer(ulong clientId)
    {
        SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(clientId);
        if (!sessionPlayerData.HasValue)
        {
            return;
        }

        Vector3 spawnPos = sessionPlayerData.Value.HasCharacterSpawned
            ? sessionPlayerData.Value.PlayerPosition
            : new Vector3(GetSpawnPositionX(clientId), 100, 0);

        GameObject playerObject = Container.Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, destroyWithScene: true);

        Player player = playerObject.GetComponent<Player>();

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));

        networkGameState.Players.Add(new PlayerListState(clientId, sessionPlayerData.Value.PlayerName));
    }

    private float GetSpawnPositionX(ulong clientId)
    {
        const float leftRightPadding = 6;
        float availableWidth = levelData.mapWidth - (2 * leftRightPadding);
        float spacing = availableWidth / Mathf.Max(connectionManager.MaxConnectedPlayers, 2f);
        int clientIdIdx = 0;
        for (; clientIdIdx < networkManager.ConnectedClientsIds.Count; clientIdIdx++)
        {
            if (networkManager.ConnectedClientsIds[clientIdIdx] == clientId)
            {
                break;
            }
        }

        return leftRightPadding + ((clientIdIdx + 1) * spacing);
    }

    private void CheckAllPlayersConnected()
    {
        if (networkManager.ConnectedClientsIds.Count == connectionManager.MaxConnectedPlayers)
        {
            networkGameState.State.Value = GameState.WaitingForPlayersReady;
        }
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        switch (networkGameState.State.Value)
        {
            case GameState.WaitingForPlayersReady:
                CheckForReady(state);
                break;
            case GameState.InGame:
                CheckForGameOver(state);
                break;
        }
    }

    private void CheckForReady(PlayerState playerState)
    {
        if (playerState == PlayerState.Ready && !Player.GetSpawnedPlayers().Any(p => p.NetworkPlayerState.State.Value != PlayerState.Ready))
        {
            networkGameState.State.Value = GameState.PregameCountdown;
            SessionManager.Instance.OnGameStarted();
        }
    }

    private void CheckForGameOver(PlayerState playerState)
    {
        if (playerState == PlayerState.Completed && !Player.GetSpawnedPlayers().Any(p => p.NetworkPlayerState.State.Value < PlayerState.Completed))
        {
            networkGameState.State.Value = GameState.Completed;
            SessionManager.Instance.OnGameEnded();
        }
    }
}
