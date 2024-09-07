using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOverUI : MonoBehaviour
{
    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive;
    private bool isPaused;

    [SerializeField] private TextMeshProUGUI gameOverText;
    [SerializeField] private Button leaveButton;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<PauseStateChangedMessage> pauseStateSubscriber)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(pauseStateSubscriber.Subscribe(OnPauseStateChanged));
    }

    private void Awake()
    {
        leaveButton.onClick.AddListener(() => connectionManager.RequestShutdown());

        UpdateActiveState(isActive);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnPauseStateChanged(PauseStateChangedMessage message)
    {
        isPaused = message.IsPaused;
        UpdateActiveState(isActive);
    }

    private void OnGameStateChanged(NetworkVariableChangedEvent<GameState> @event)
    {
        if (@event.newValue == GameState.Completed && @event.previousValue != GameState.Completed)
        {
            UpdateActiveState(isActive: true);
            Debug.Log(networkGameState.TimeRemaining.Value);
            gameOverText.text = networkGameState.TimeRemaining.Value == 0 ? "TIMES UP!" : "MATCH COMPLETE!";
        }
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;
        gameObject.SetActive(isActive && !isPaused);
    }
}
