using Unity.Netcode;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    private void Awake()
    {
        DontDestroyOnLoad(this);
        Instance = this;
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += OnConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void OnConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if (NetworkManager.Singleton.ConnectedClientsIds.Count >= 2)
        {
            response.Approved = false;
            response.Reason = "Game is full";
        }

        if (GameManager.Instance != null && GameManager.Instance.State.Value != GameState.WaitingForPlayers)
        {
            response.Approved = false;
            response.Reason = "Game has already started";
        }

        response.Approved = true;
    }
}
