using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using TMPro;
using UnityEngine;
using VContainer;

public class WaitingForReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;

    [SerializeField] private TextMeshProUGUI waitingForPlayerReadyText;
    [SerializeField] private PlayerGameEvent localPlayerSpawnedEvent;

    private void Awake()
    {
        localPlayerSpawnedEvent.RegisterListener(OnLocalPlayerSpawned);
    }

    private void Start()
    {
        Hide();

        subscriptions ??= new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);

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
            waitingForPlayerReadyText.text = "WAITING FOR PLAYERS TO READY UP.";
        }
        else
        {
            Hide();
        }
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
