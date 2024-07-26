using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.UGS;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);
    }

    private void Start()
    {
        BufferedMessageChannel<ConnectStatus>.Instance.Subscribe(OnConnectStatusMessage);
        LobbyManager.Instance.OnLobbyCreating += OnLobbyCreating;
        LobbyManager.Instance.OnLobbyCreationFailed += OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoiningLobby += OnJoiningLobby;
        LobbyManager.Instance.OnLobbyJoinFailed += OnLobbyJoinFailed;

        Hide();
    }

    private void OnDestroy()
    {
        BufferedMessageChannel<ConnectStatus>.Instance.Unsubscribe(OnConnectStatusMessage);
        LobbyManager.Instance.OnLobbyCreating -= OnLobbyCreating;
        LobbyManager.Instance.OnLobbyCreationFailed -= OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoiningLobby -= OnJoiningLobby;
        LobbyManager.Instance.OnLobbyJoinFailed -= OnLobbyJoinFailed;
    }

    private void OnConnectStatusMessage(ConnectStatus status)
    {
        Debug.Log("STATUS: " + status);
        switch (status)
        {
            case ConnectStatus.ServerFull:
                Show("Game is full.", closable: true);
                break;
            case ConnectStatus.GenericDisconnect:
                Show("The connection to the host was lost.", closable: true);
                break;
            case ConnectStatus.HostEndedSession:
                Show("The host has ended the game session.", closable: true);
                break;
            case ConnectStatus.StartHostFailed:
                Show("Starting host failed.", closable: true);
                break;
            case ConnectStatus.StartClientFailed:
                Show("Starting client failed.", closable: true);
                break;
        }
    }

    private void OnLobbyCreating() => Show("Creating match...");

    private void OnLobbyCreationFailed() => Show("Failed to create match.", closable: true);

    private void OnJoiningLobby() => Show("Joining match...");

    private void OnLobbyJoinFailed() => Show("Failed to join match.", closable: true);

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
