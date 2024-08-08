using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PregameCountdownUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    [SerializeField] private Text pregameTimeText;

    private void Start()
    {
        networkGameState.State.OnValueChanged += HandleGameStateChanged;
        networkGameState.PregameTimeRemaining.OnValueChanged += HandlePregameTimeRemainingChanged;

        gameObject.SetActive(false);
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isPregameCountdown = newState == GameState.PregameCountdown;
        gameObject.SetActive(isPregameCountdown);

        // TODO: There must be a better way of doing this?
        HandlePregameTimeRemainingChanged(0, networkGameState.PregameTimeRemaining.Value);
    }

    private void HandlePregameTimeRemainingChanged(int previousTime, int newTime)
    {
        string seconds = newTime.ToString("00");
        pregameTimeText.text = seconds;
    }
}
