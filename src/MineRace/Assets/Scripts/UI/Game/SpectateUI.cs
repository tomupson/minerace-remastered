using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SpectateUI : MonoBehaviour
{
    private PlayerInputActions inputActions;

    [SerializeField] private Text spectateText;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Spectate.performed += OnSpectatePerformed;
    }

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
        if (newState == GameState.Completed)
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
        if (newState != PlayerState.Completed)
        {
            return;
        }

        bool isGameRunning = ServerGameState.Instance.State.Value == GameState.InGame;
        gameObject.SetActive(isGameRunning);
    }

    private void OnSpectatePerformed(InputAction.CallbackContext context)
    {
        bool localPlayerNotCompleted = Player.LocalPlayer.State.Value != PlayerState.Completed;
        bool gameNotRunning = ServerGameState.Instance.State.Value != GameState.InGame;
        if (localPlayerNotCompleted || gameNotRunning)
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);

        Player.LocalPlayer.Spectate(otherPlayer);

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
