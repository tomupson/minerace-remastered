using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class PregameCountdownUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;

    [SerializeField] private TextMeshProUGUI pregameTimeText;

    private void Start()
    {
        gameObject.SetActive(false);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
        subscriptions.Add(networkGameState.PregameTimeRemaining.Subscribe(HandlePregameTimeRemainingChanged));
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnGameStateChanged(GameState state)
    {
        bool isPregameCountdown = state == GameState.PregameCountdown;
        gameObject.SetActive(isPregameCountdown);
    }

    private void HandlePregameTimeRemainingChanged(int timeRemaining)
    {
        string seconds = timeRemaining.ToString("00");
        pregameTimeText.text = seconds;
    }
}
