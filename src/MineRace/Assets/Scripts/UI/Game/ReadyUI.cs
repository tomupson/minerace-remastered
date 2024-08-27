using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class ReadyUI : MonoBehaviour
{
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;
    private Player player;

    [SerializeField] private Button readyButton;
    [SerializeField] private PlayerGameEvent localPlayerSpawnedEvent;

    private void Awake()
    {
        localPlayerSpawnedEvent.RegisterListener(OnLocalPlayerSpawned);

        readyButton.onClick.AddListener(() =>
        {
            Hide();
            player.Ready();
        });
    }

    private void Start()
    {
        Hide();

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);

        subscriptions?.Dispose();
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        this.player = player;
    }

    private void OnGameStateChanged(GameState state)
    {
        bool isReadyUp = state == GameState.WaitingForPlayersReady;
        gameObject.SetActive(isReadyUp);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
