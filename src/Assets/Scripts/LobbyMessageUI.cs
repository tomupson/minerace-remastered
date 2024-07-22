using Unity.Netcode;
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
        ConnectionManager.Instance.OnConnecting += OnConnecting;
        ConnectionManager.Instance.OnConnectionFailed += OnConnectionFailed;
        LobbyManager.Instance.OnLobbyCreating += OnLobbyCreating;
        LobbyManager.Instance.OnLobbyCreationFailed += OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoiningLobby += OnJoiningLobby;
        LobbyManager.Instance.OnLobbyJoinFailed += OnLobbyJoinFailed;

        Hide();
    }

    private void OnDestroy()
    {
        ConnectionManager.Instance.OnConnecting -= OnConnecting;
        ConnectionManager.Instance.OnConnectionFailed -= OnConnectionFailed;
        LobbyManager.Instance.OnLobbyCreating -= OnLobbyCreating;
        LobbyManager.Instance.OnLobbyCreationFailed -= OnLobbyCreationFailed;
        LobbyManager.Instance.OnJoiningLobby -= OnJoiningLobby;
        LobbyManager.Instance.OnLobbyJoinFailed -= OnLobbyJoinFailed;
    }

    private void OnConnecting() => Show("Connecting...");

    private void OnConnectionFailed()
    {
        string message = NetworkManager.Singleton.DisconnectReason;
        if (string.IsNullOrWhiteSpace(message))
        {
            message = "Failed to connect.";
        }

        Show(message, closable: true);
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
