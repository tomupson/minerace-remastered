using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOverUI : MonoBehaviour
{
    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly NetworkGameState networkGameState;

    private DisposableGroup subscriptions;

    [SerializeField] private Text gameOverText;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        leaveButton.onClick.AddListener(() => connectionManager.RequestShutdown());
    }

    private void Start()
    {
        gameObject.SetActive(false);

        subscriptions = new DisposableGroup();
        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnGameStateChanged(NetworkVariableChangedEvent<GameState> @event)
    {
        if (@event.newValue == GameState.Completed && @event.previousValue != GameState.Completed)
        {
            gameObject.SetActive(true);
            Debug.Log(networkGameState.TimeRemaining.Value);
            gameOverText.text = networkGameState.TimeRemaining.Value == 0 ? "TIMES UP!" : "MATCH COMPLETE!";
        }
    }
}
