using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class WaitingForReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    [SerializeField] private Text waitingForPlayerReadyText;

    private void Awake()
    {
        Player.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
    }

    private void Start()
    {
        networkGameState.State.OnValueChanged += HandleGameStateChanged;

        Hide();
    }

    private void OnDestroy()
    {
        Player.OnLocalPlayerSpawned -= OnLocalPlayerSpawned;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.PregameCountdown)
        {
            Hide();
        }
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        player.NetworkPlayerState.State.OnValueChanged -= HandlePlayerStateChanged;
        player.NetworkPlayerState.State.OnValueChanged += HandlePlayerStateChanged;
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Ready)
        {
            gameObject.SetActive(true);
            waitingForPlayerReadyText.text = $"WAITING FOR PLAYERS TO READY UP.";
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
