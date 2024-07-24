using UnityEngine;
using UnityEngine.UI;

public class PointsUI : MonoBehaviour
{
    private Player spectatingPlayer;

    [SerializeField] private Text pointsText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text spectatingText;

    private void Start()
    {
        GameManager.Instance.State.OnValueChanged += HandleGameStateChanged;
        GameManager.Instance.TimeRemaining.OnValueChanged += HandleTimeRemainingChanged;
        Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;

        Hide();
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawned -= OnAnyPlayerSpawned;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isInGame = newState == GameState.InGame;
        gameObject.SetActive(isInGame);
    }

    private void HandleTimeRemainingChanged(int previousTime, int newTime)
    {
        string minutes = Mathf.Floor(newTime / 60).ToString("00");
        string seconds = (newTime % 60).ToString("00");
        timeText.text = $"{minutes}:{seconds} remaining";
    }

    private void OnAnyPlayerSpawned(Player player)
    {
        if (Player.LocalPlayer != null)
        {
            Player.LocalPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;
            Player.LocalPlayer.Points.OnValueChanged += HandlePlayerPointsChanged;

            Player.LocalPlayer.State.OnValueChanged -= HandlePlayerStateChanged;
            Player.LocalPlayer.State.OnValueChanged += HandlePlayerStateChanged;

            Player.LocalPlayer.OnSpectating -= OnPlayerSpectating;
            Player.LocalPlayer.OnSpectating += OnPlayerSpectating;
        }
    }

    private void HandlePlayerPointsChanged(int previousPoints, int newPoints)
    {
        pointsText.text = $"POINTS: {newPoints}";
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Completed && GameManager.Instance.State.Value != GameState.InGame)
        {
            Hide();
        }
    }

    private void OnPlayerSpectating(Player spectatingPlayer)
    {
        Player.LocalPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;

        if (this.spectatingPlayer != null)
        {
            this.spectatingPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;
        }

        this.spectatingPlayer = spectatingPlayer;
        this.spectatingPlayer.Points.OnValueChanged += HandlePlayerPointsChanged;
        HandlePlayerPointsChanged(0, this.spectatingPlayer.Points.Value);

        spectatingText.gameObject.SetActive(true);
        spectatingText.text = $"YOU ARE SPECTATING: {this.spectatingPlayer.Username.Value}";
    }

    private void Hide()
    {
        spectatingText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
