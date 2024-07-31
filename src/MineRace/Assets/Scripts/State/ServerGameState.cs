using System.Collections.Generic;
using System.Linq;
using MineRace.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

[RequireComponent(typeof(NetworkHooks))]
public class ServerGameState : LifetimeScope
{
    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly NetworkManager networkManager;

    public static ServerGameState Instance { get; private set; }

    private Player[] players;
    private float ticker = 1f;

    [SerializeField] private ChatManager chatManager;
    [SerializeField] private PauseManager pauseManager;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private LevelData levelData;

    [Header("Game Settings")]
    [SerializeField] private int gameTime = 300;
    [SerializeField] private int preGameCountdownTime = 10;

    public NetworkVariable<GameState> State { get; } = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public NetworkVariable<int> PregameTimeRemaining { get; } = new NetworkVariable<int>();

    public NetworkVariable<int> TimeRemaining { get; } = new NetworkVariable<int>();

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterComponent(chatManager);
        builder.RegisterComponent(pauseManager);
    }

    protected override void Awake()
    {
        Instance = this;

        NetworkHooks networkHooks = GetComponent<NetworkHooks>();
        networkHooks.OnNetworkSpawnHook += OnNetworkSpawn;
        networkHooks.OnNetworkDespawnHook += OnNetworkDespawn;
    }

    private void Update()
    {
        if (State.Value == GameState.PregameCountdown)
        {
            ticker -= Time.deltaTime;
            if (ticker <= 0f)
            {
                PregameTimeRemaining.Value--;
                if (PregameTimeRemaining.Value == 0)
                {
                    State.Value = GameState.InGame;
                }

                ticker = 1f;
            }
        }

        if (State.Value == GameState.InGame)
        {
            ticker -= Time.deltaTime;
            if (ticker <= 0f)
            {
                TimeRemaining.Value--;
                if (TimeRemaining.Value == 0)
                {
                    State.Value = GameState.Completed;
                }

                ticker = 1f;
            }
        }
    }

    private void OnNetworkSpawn()
    {
        if (!networkManager.IsServer)
        {
            enabled = false;
            return;
        }

        TimeRemaining.Value = gameTime;
        PregameTimeRemaining.Value = preGameCountdownTime;
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
            State.Value = GameState.WaitingForPlayersReady;
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
    private void SpawnPlayer(ulong clientId)
    {
        GameObject playerObject = Instantiate(playerPrefab);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, destroyWithScene: true);
        playerObject.transform.position = new Vector3(levelData.mapWidth / 2 + 12 * ((int)clientId * 2 - 1), 100, 0);

        playerObject.GetComponent<Player>().State.OnValueChanged += OnPlayerStateChanged;

        Container.InjectGameObject(playerObject);
    }

    private void OnPlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (State.Value)
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
        if (playerState == PlayerState.Ready && !players.Any(p => p.State.Value != PlayerState.Ready))
        {
            State.Value = GameState.PregameCountdown;
        }
    }

    private void CheckForGameOver(PlayerState playerState)
    {
        if (playerState == PlayerState.Completed && !players.Any(p => p.State.Value < PlayerState.Completed))
        {
            State.Value = GameState.Completed;
        }
    }
}
