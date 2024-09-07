using System.Collections;
using JetBrains.Annotations;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class WaitingForPlayersUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private bool isActive = true;
    private bool isPaused;
    private Coroutine waitForPlayersCoroutine;

    [SerializeField] private TextMeshProUGUI waitingForPlayersText;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<PauseStateChangedMessage> pauseStateSubscriber)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(pauseStateSubscriber.Subscribe(OnPauseStateChanged));

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

    private void OnGameStateChanged(GameState state)
    {
        bool isWaitingForPlayers = state == GameState.WaitingForPlayers;
        UpdateActiveState(isWaitingForPlayers);
    }

    private IEnumerator WaitForPlayers()
    {
        int index = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            waitingForPlayersText.text = $"WAITING FOR PLAYERS{new string('.', index)}";
            index++;
            index %= 4;
        }
    }

    private void UpdateActiveState(bool isActive)
    {
        this.isActive = isActive;

        bool activeState = isActive && !isPaused;
        gameObject.SetActive(activeState);

        if (!activeState && waitForPlayersCoroutine != null)
        {
            StopCoroutine(waitForPlayersCoroutine);
            waitForPlayersCoroutine = null;
        }
        else if (activeState && waitForPlayersCoroutine == null)
        {
            waitForPlayersCoroutine = StartCoroutine(WaitForPlayers());
        }
    }
}
