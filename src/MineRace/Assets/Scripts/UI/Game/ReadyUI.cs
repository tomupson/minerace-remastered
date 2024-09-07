using JetBrains.Annotations;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive;
    private bool isPaused;
    private Player player;

    [SerializeField] private Button readyButton;
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

        readyButton.onClick.AddListener(() =>
        {
            UpdateActiveState(isActive: false);
            player.Ready();
        });

        UpdateActiveState(isActive);

        subscriptions = new DisposableGroup();
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
        this.player = player;
    }

    private void OnGameStateChanged(GameState state)
    {
        bool isReadyUp = state == GameState.WaitingForPlayersReady;
        UpdateActiveState(isReadyUp);
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;
        gameObject.SetActive(isActive && !isPaused);
    }
}
