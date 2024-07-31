using UnityEngine;
using UnityEngine.UI;

public class PregameCountdownUI : MonoBehaviour
{
    [SerializeField] private Text pregameTimeText;

    private void Start()
    {
        ServerGameState.Instance.State.OnValueChanged += HandleGameStateChanged;
        ServerGameState.Instance.PregameTimeRemaining.OnValueChanged += HandlePregameTimeRemainingChanged;

        gameObject.SetActive(false);
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isPregameCountdown = newState == GameState.PregameCountdown;
        gameObject.SetActive(isPregameCountdown);
    }

    private void HandlePregameTimeRemainingChanged(int previousTime, int newTime)
    {
        string seconds = newTime.ToString("00");
        pregameTimeText.text = seconds;
    }
}
