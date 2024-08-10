using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class WaitingForReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;

    [SerializeField] private Text waitingForPlayerReadyText;

    private void Awake()
    {
        Player.OnLocalPlayerSpawned += OnLocalPlayerSpawned;
    }

    private void Start()
    {
        Hide();

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        Player.OnLocalPlayerSpawned -= OnLocalPlayerSpawned;

        subscriptions?.Dispose();
    }

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.PregameCountdown)
        {
            Hide();
        }
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        subscriptions ??= new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state == PlayerState.Ready)
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
