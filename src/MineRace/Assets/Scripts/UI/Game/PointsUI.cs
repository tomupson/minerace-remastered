using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PointsUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private Player player;
    private Player spectatingPlayer;

    [SerializeField] private Text pointsText;
    [SerializeField] private Text timeText;
    [SerializeField] private Text spectatingText;

    private void Awake()
    {
        Player.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
    }

    private void Start()
    {
        networkGameState.State.OnValueChanged += HandleGameStateChanged;
        networkGameState.TimeRemaining.OnValueChanged += HandleTimeRemainingChanged;

        Hide();
    }

    private void OnDestroy()
    {
        Player.OnLocalPlayerSpawned -= OnLocalPlayerSpawned;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isInGame = newState == GameState.InGame;
        gameObject.SetActive(isInGame);

        // TODO: There must be a better way of doing this?
        HandleTimeRemainingChanged(0, networkGameState.TimeRemaining.Value);
    }

    private void HandleTimeRemainingChanged(int previousTime, int newTime)
    {
        string minutes = Mathf.Floor(newTime / 60).ToString("00");
        string seconds = (newTime % 60).ToString("00");
        timeText.text = $"{minutes}:{seconds} remaining";
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        this.player = player;
        this.player.NetworkPlayerState.Points.OnValueChanged += HandlePlayerPointsChanged;
        this.player.NetworkPlayerState.State.OnValueChanged += HandlePlayerStateChanged;
        this.player.OnSpectating += OnPlayerSpectating;

        spectatingPlayer = player;
    }

    private void HandlePlayerPointsChanged(int previousPoints, int newPoints)
    {
        pointsText.text = $"POINTS: {newPoints}";
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Completed && networkGameState.State.Value != GameState.InGame)
        {
            Hide();
        }
    }

    private void OnPlayerSpectating(Player spectatingPlayer)
    {
        if (this.spectatingPlayer != null)
        {
            this.spectatingPlayer.NetworkPlayerState.Points.OnValueChanged -= HandlePlayerPointsChanged;
        }

        this.spectatingPlayer = spectatingPlayer;
        this.spectatingPlayer.NetworkPlayerState.Points.OnValueChanged += HandlePlayerPointsChanged;
        HandlePlayerPointsChanged(0, this.spectatingPlayer.NetworkPlayerState.Points.Value);

        spectatingText.gameObject.SetActive(true);
        spectatingText.text = $"YOU ARE SPECTATING: {this.spectatingPlayer.NetworkPlayerState.Username.Value}";
    }

    private void Hide()
    {
        spectatingText.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
}
