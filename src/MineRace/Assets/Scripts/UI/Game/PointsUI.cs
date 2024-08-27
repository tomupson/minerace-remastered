using System;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PointsUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private IDisposable pointsSubscription;
    private Player player;
    private Player spectatingPlayer;

    [SerializeField] private Text pointsText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text spectatingText;
    [SerializeField] private PlayerGameEvent localPlayerSpawnedEvent;

    private void Awake()
    {
        localPlayerSpawnedEvent.RegisterListener(OnLocalPlayerSpawned);
    }

    private void Start()
    {
        spectatingText.gameObject.SetActive(false);
        gameObject.SetActive(false);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
        subscriptions.Add(networkGameState.TimeRemaining.Subscribe(OnTimeRemainingChanged));
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);

        subscriptions?.Dispose();
        pointsSubscription?.Dispose();
    }

    private void OnGameStateChanged(GameState state)
    {
        bool isInGame = state == GameState.InGame;
        gameObject.SetActive(isInGame);
    }

    private void OnTimeRemainingChanged(int timeRemaining)
    {
        string minutes = Mathf.Floor(timeRemaining / 60).ToString("00");
        string seconds = (timeRemaining % 60).ToString("00");
        timeText.text = $"{minutes}:{seconds} remaining";
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        this.player = player;
        this.player.OnSpectating += OnPlayerSpectating;

        SetupSpectatingPlayer(player);
    }

    private void OnPlayerSpectating(Player spectatingPlayer)
    {
        SetupSpectatingPlayer(spectatingPlayer);

        spectatingText.gameObject.SetActive(true);
        spectatingText.text = $"YOU ARE SPECTATING: {this.spectatingPlayer.NetworkPlayerState.Username.Value}";
    }

    private void OnPlayerPointsChanged(int points)
    {
        pointsText.text = $"POINTS: {points}";
    }

    private void SetupSpectatingPlayer(Player spectatingPlayer)
    {
        this.spectatingPlayer = spectatingPlayer;

        pointsSubscription?.Dispose();
        pointsSubscription = this.spectatingPlayer.NetworkPlayerState.Points.Subscribe(OnPlayerPointsChanged);
    }
}
