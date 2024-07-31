using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForReadyUI : MonoBehaviour
{
    [SerializeField] private Text waitingForPlayerReadyText;

    private void Start()
    {
        ServerGameState.Instance.State.OnValueChanged += HandleGameStateChanged;
        Player.OnAnyPlayerSpawned += OnAnyPlayerSpawned;

        Hide();
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawned -= OnAnyPlayerSpawned;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.PregameCountdown)
        {
            Hide();
        }
    }

    private void OnAnyPlayerSpawned(Player player)
    {
        if (Player.LocalPlayer != null)
        {
            Player.LocalPlayer.State.OnValueChanged -= HandlePlayerStateChanged;
            Player.LocalPlayer.State.OnValueChanged += HandlePlayerStateChanged;
        }
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Ready)
        {
            gameObject.SetActive(true);

            Player[] players = FindObjectsOfType<Player>();
            Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);
            waitingForPlayerReadyText.text = $"WAITING FOR '{otherPlayer.Username.Value}' TO READY UP.";
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
