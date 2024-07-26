using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player[] players;
    private float ticker = 1f;

    [SerializeField] private GameObject playerPrefab;

    [Header("Game Settings")]
    [SerializeField] private int gameTime = 300;
    [SerializeField] private int preGameCountdownTime = 10;

    public NetworkVariable<GameState> State { get; } = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public NetworkVariable<int> PregameTimeRemaining { get; } = new NetworkVariable<int>();

    public NetworkVariable<int> TimeRemaining { get; } = new NetworkVariable<int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        if (State.Value == GameState.WaitingForPlayersReady)
        {
            if (!players.Any(p => p.State.Value != PlayerState.Ready))
            {
                State.Value = GameState.PregameCountdown;
            }
        }

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

    public override void OnNetworkSpawn()
    {
        TimeRemaining.Value = gameTime;
        PregameTimeRemaining.Value = preGameCountdownTime;
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadEventCompleted;
        }
    }

    public void CompleteGame()
    {
        GameOverServerRpc();
    }

    private void OnClientConnected(ulong connectedClientId)
    {
        SpawnPlayer(connectedClientId);
        players = FindObjectsOfType<Player>();

        if (NetworkManager.ConnectedClientsIds.Count == 2)
        {
            State.Value = GameState.WaitingForPlayersReady;
        }
    }

    private void OnSceneLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        // TODO: Check if Game scene
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
    }

    [ServerRpc(RequireOwnership = false)]
    private void GameOverServerRpc()
    {
        State.Value = GameState.Completed;
    }
}
