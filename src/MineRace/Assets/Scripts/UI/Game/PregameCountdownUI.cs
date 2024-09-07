using JetBrains.Annotations;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class PregameCountdownUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive;
    private bool isPaused;

    [SerializeField] private TextMeshProUGUI pregameTimeText;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<PauseStateChangedMessage> pauseStateSubscriber)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(pauseStateSubscriber.Subscribe(OnPauseStateChanged));
    }

    private void Awake()
    {
        UpdateActiveState(isActive);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
        subscriptions.Add(networkGameState.PregameTimeRemaining.Subscribe(HandlePregameTimeRemainingChanged));
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

    private void OnGameStateChanged(GameState state)
    {
        bool isPregameCountdown = state == GameState.PregameCountdown;
        UpdateActiveState(isPregameCountdown);
    }

    private void HandlePregameTimeRemainingChanged(int timeRemaining)
    {
        string seconds = timeRemaining.ToString("00");
        pregameTimeText.text = seconds;
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;
        gameObject.SetActive(isActive && !isPaused);
    }
}
