using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private Player localPlayer;

    [SerializeField] private Button readyButton;

    private void Awake()
    {
        Player.OnLocalPlayerSpawned += OnLocalPlayerSpawned;

        readyButton.onClick.AddListener(() =>
        {
            Hide();
            localPlayer.Ready();
        });
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

    private void OnLocalPlayerSpawned(Player player)
    {
        localPlayer = player;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isReadyUp = newState == GameState.WaitingForPlayersReady;
        gameObject.SetActive(isReadyUp);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
