using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LobbyMessageUI : MonoBehaviour
{
    private DisposableGroup subscriptions;

    [SerializeField] private Text messageText;
    [SerializeField] private Button closeButton;

    [Inject, UsedImplicitly]
    private void InjectDependencies(
        ISubscriber<ConnectStatus> connectStatusSubscriber,
        ISubscriber<ReconnectMessage> reconnectSubscriber,
        ISubscriber<LobbyStatus> lobbyStatusSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(connectStatusSubscriber.Subscribe(OnConnectStatus));
        subscriptions.Add(reconnectSubscriber.Subscribe(OnReconnectMessage));
        subscriptions.Add(lobbyStatusSubscriber.Subscribe(OnLobbyStatus));
    }

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        Hide();
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnConnectStatus(ConnectStatus status)
    {
        switch (status)
        {
            case ConnectStatus.ServerFull:
                Show("Game is full.", closable: true);
                break;
            case ConnectStatus.StartHostFailed:
                Show("Starting host failed.", closable: true);
                break;
            case ConnectStatus.StartClientFailed:
                Show("Starting client failed.", closable: true);
                break;
        }
    }

    private void OnReconnectMessage(ReconnectMessage message)
    {
        if (message.CurrentAttempt == message.MaxAttempt)
        {
            Hide();
            return;
        }

        Show($"Attempting to reconnect...\nAttempt {message.CurrentAttempt + 1}/{message.MaxAttempt}");
    }

    private void OnLobbyStatus(LobbyStatus status)
    {
        switch (status)
        {
            case LobbyStatus.Creating:
                Show("Creating match...");
                break;
            case LobbyStatus.CreationFailed:
                Show("Failed to create match.", closable: true);
                break;
            case LobbyStatus.Joining:
                Show("Joining match...");
                break;
            case LobbyStatus.JoinFailed:
                Show("Failed to join match.", closable: true);
                break;
        }
    }

    private void Show(string message, bool closable = false)
    {
        messageText.text = message;
        gameObject.SetActive(true);
        closeButton.gameObject.SetActive(closable);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
