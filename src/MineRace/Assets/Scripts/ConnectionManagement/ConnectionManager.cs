using MineRace.ConnectionManagement.States;
using MineRace.Infrastructure;
using MineRace.UGS;
using Unity.Netcode;
using UnityEngine;

namespace MineRace.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        private ConnectionState currentState;

        public static ConnectionManager Instance { get; private set; }

        [SerializeField] private int numReconnectAttempts = 2;

        public int NumReconnectAttempts => numReconnectAttempts;

        public int MaxConnectedPlayers = 2;

        internal OfflineState OfflineState { get; private set; }
        internal ClientConnectingState ClientConnectingState { get; private set; }
        internal ClientConnectedState ClientConnectedState { get; private set; }
        internal ClientReconnectingState ClientReconnectingState { get; private set; }
        internal StartingHostState StartingHostState { get; private set; }
        internal HostingState HostingState { get; private set; }

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }

        private void Start()
        {
            OfflineState = new OfflineState(this, BufferedMessageChannel<ConnectStatus>.Instance, LobbyManager.Instance);
            ClientConnectingState = new ClientConnectingState(this, BufferedMessageChannel<ConnectStatus>.Instance, LobbyManager.Instance);
            ClientConnectedState = new ClientConnectedState(this, BufferedMessageChannel<ConnectStatus>.Instance);
            ClientReconnectingState = new ClientReconnectingState(this, BufferedMessageChannel<ConnectStatus>.Instance, LobbyManager.Instance, MessageChannel<ReconnectMessage>.Instance);
            StartingHostState = new StartingHostState(this, BufferedMessageChannel<ConnectStatus>.Instance, LobbyManager.Instance);
            HostingState = new HostingState(this, BufferedMessageChannel<ConnectStatus>.Instance);

            currentState = OfflineState;

            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnectCallback;
            NetworkManager.Singleton.OnServerStarted += OnServerStarted;
            NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
            NetworkManager.Singleton.OnTransportFailure += OnTransportFailure;
            NetworkManager.Singleton.OnServerStopped += OnServerStopped;
        }

        private void OnDestroy()
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnClientDisconnectCallback;
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
            NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
            NetworkManager.Singleton.OnTransportFailure -= OnTransportFailure;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {currentState.GetType().Name} to {nextState.GetType().Name}.");

            currentState?.Exit();
            currentState = nextState;
            currentState.Enter();
        }

        public void StartClient() =>
            currentState.StartClient();

        public void StartHost() =>
            currentState.StartHost();

        public void RequestShutdown() =>
            currentState.OnUserRequestedShutdown();

        private void OnClientConnectedCallback(ulong clientId) =>
            currentState.OnClientConnected(clientId);

        private void OnClientDisconnectCallback(ulong clientId) =>
            currentState.OnClientDisconnect(clientId);

        private void OnServerStarted() =>
            currentState.OnServerStarted();

        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response) =>
            currentState.ApprovalCheck(request, response);

        private void OnTransportFailure() =>
            currentState.OnTransportFailure();

        private void OnServerStopped(bool _) =>
            currentState.OnServerStopped();
    }
}
