using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles functionalities that all players should experience e.g. the game countdown timer.
/// </summary>
public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private Player[] players;

    [SerializeField] private GameObject playerPrefab;

    [Header("Game Settings")]
    [SerializeField] private int gameTime = 300;
    [SerializeField] private int preGameCountdownTime = 10;

    public NetworkVariable<GameState> State { get; } = new NetworkVariable<GameState>(GameState.PreGame);

    public NetworkVariable<int> PreGameTimeRemaining { get; } = new NetworkVariable<int>();

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

        // TODO: Move Coroutines to here
    }

    public override void OnNetworkSpawn()
    {
        TimeRemaining.Value = gameTime;
        PreGameTimeRemaining.Value = preGameCountdownTime;
        if (IsServer)
        {
            NetworkManager.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoadEventCompleted;
        }
    }

    private void OnClientConnected(ulong connectedClientId)
    {
        SpawnPlayer(connectedClientId);

        if (NetworkManager.ConnectedClientsIds.Count == 2)
        {
            players = FindObjectsOfType<Player>();

            foreach (Player player in players)
            {
                player.SetModeServerRpc(PlayerState.ReadyUp);
            }

            StartCoroutine(WaitForReady());
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

    private IEnumerator WaitForReady()
    {
        while (players.Count(p => p.State.Value == PlayerState.WaitingForPlayerReady) < 2)
        {
            yield return null;
        }

        foreach (Player player in players)
        {
            player.SetModeServerRpc(PlayerState.PregameCountdown);
        }

        StartCoroutine(PreGameCountdown());
    }

    private IEnumerator PreGameCountdown()
    {
        while (PreGameTimeRemaining.Value > 0)
        {
            yield return new WaitForSeconds(1);
            PreGameTimeRemaining.Value--;
            if (State.Value == GameState.PreGame && PreGameTimeRemaining.Value == 0)
            {
                State.Value = GameState.Playing;
            }
        }

        foreach (Player player in players)
        {
            player.SetModeServerRpc(PlayerState.InGame);
        }

        StartCoroutine(GameCountdown());
    }

    private IEnumerator GameCountdown()
    {
        while (TimeRemaining.Value > 0)
        {
            yield return new WaitForSeconds(1);
            TimeRemaining.Value--;
            if (State.Value == GameState.Playing && TimeRemaining.Value == 0)
            {
                State.Value = GameState.Completed;

                foreach (Player player in players)
                {
                    player.SetModeServerRpc(PlayerState.Completed);
                }
            }
        }

        yield return null;
    }

    [ServerRpc]
    public void GameOverServerRpc()
    {
        State.Value = GameState.Completed;
        foreach (Player player in players)
        {
            player.SetModeServerRpc(PlayerState.GameOver);
        }
    }
}
