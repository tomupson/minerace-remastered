using JetBrains.Annotations;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class WaitingForReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive;
    private bool isPaused;

    [SerializeField] private TextMeshProUGUI waitingForPlayerReadyText;
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
    }

    private void Start()
    {
        UpdateActiveState(isActive);
        waitingForPlayerReadyText.text = "WAITING FOR PLAYERS TO READY UP.";

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);

        subscriptions?.Dispose();
    }

    private void OnPauseStateChanged(PauseStateChangedMessage message)
    {
        isPaused = message.IsPaused;
        UpdateActiveState(isActive);
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.PregameCountdown)
        {
            UpdateActiveState(isActive: false);
        }
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        bool isReady = state == PlayerState.Ready;
        UpdateActiveState(isReady);
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;
        gameObject.SetActive(isActive && !isPaused);
    }
}
