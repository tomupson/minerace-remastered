using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class SpectateUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private Player player;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private Text spectateText;

    private void Awake()
    {
        Player.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
        inputReader.OnSpectateHook += OnSpectate;
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
        if (newState == GameState.Completed)
        {
            Hide();
        }
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        this.player = player;
        this.player.NetworkPlayerState.State.OnValueChanged += HandlePlayerStateChanged;
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState != PlayerState.Completed)
        {
            return;
        }

        bool isGameRunning = networkGameState.State.Value == GameState.InGame;
        gameObject.SetActive(isGameRunning);
    }

    private void OnSpectate()
    {
        bool localPlayerNotCompleted = player.NetworkPlayerState.State.Value != PlayerState.Completed;
        bool gameNotRunning = networkGameState.State.Value != GameState.InGame;
        if (localPlayerNotCompleted || gameNotRunning)
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);

        player.Spectate(otherPlayer);

        Hide();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
