using System.Linq;
using JetBrains.Annotations;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class SpectateUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive;
    private bool isPaused;
    private Player player;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private TextMeshProUGUI spectateText;
    [SerializeField] private PlayerGameEvent localPlayerSpawnedEvent;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<PauseStateChangedMessage> pauseStateSubscriber)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(pauseStateSubscriber.Subscribe(OnPauseStateChanged));
    }

    private void Awake()
    {
        localPlayerSpawnedEvent.RegisterListener(OnLocalPlayerSpawned);
        inputReader.OnSpectateHook += OnSpectate;

        UpdateActiveState(isActive);

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);
        inputReader.OnSpectateHook -= OnSpectate;

        subscriptions?.Dispose();
    }

    private void OnPauseStateChanged(PauseStateChangedMessage message)
    {
        isPaused = message.IsPaused;
        UpdateActiveState(isActive);
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        this.player = player;

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(this.player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.Completed)
        {
            UpdateActiveState(isActive: false);
        }
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state != PlayerState.Completed)
        {
            return;
        }

        bool isGameRunning = networkGameState.State.Value == GameState.InGame;
        UpdateActiveState(isGameRunning);
    }

    private void OnSpectate()
    {
        bool localPlayerNotCompleted = player.NetworkPlayerState.State.Value != PlayerState.Completed;
        bool gameNotRunning = networkGameState.State.Value != GameState.InGame;
        if (localPlayerNotCompleted || gameNotRunning)
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);

        player.Spectate(otherPlayer);

        UpdateActiveState(isActive: false);
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;
        gameObject.SetActive(isActive && !isPaused);
    }
}
