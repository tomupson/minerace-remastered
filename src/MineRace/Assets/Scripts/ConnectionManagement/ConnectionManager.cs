using MineRace.ConnectionManagement.States;
using Unity.Netcode;
using UnityEngine;
using VContainer;

namespace MineRace.ConnectionManagement
{
    public class ConnectionManager : MonoBehaviour
    {
        [Inject] private readonly NetworkManager networkManager;
        [Inject] private readonly IObjectResolver container;

        private ConnectionState currentState;

        [SerializeField] private int numReconnectAttempts = 2;
        [SerializeField] private int maxConnectedPlayers = 2;

        public int NumReconnectAttempts => numReconnectAttempts;

        public int MaxConnectedPlayers => maxConnectedPlayers;

        internal OfflineState OfflineState { get; } = new OfflineState();
        internal ClientConnectingState ClientConnectingState { get; } = new ClientConnectingState();
        internal ClientConnectedState ClientConnectedState { get; } = new ClientConnectedState();
        internal ClientReconnectingState ClientReconnectingState { get; } = new ClientReconnectingState();
        internal StartingHostState StartingHostState { get; } = new StartingHostState();
        internal HostingState HostingState { get; } = new HostingState();

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            container.Inject(OfflineState);
            container.Inject(ClientConnectingState);
            container.Inject(ClientConnectedState);
            container.Inject(ClientReconnectingState);
            container.Inject(StartingHostState);
            container.Inject(HostingState);

            currentState = OfflineState;

            networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            networkManager.OnClientDisconnectCallback += OnClientDisconnectCallback;
            networkManager.OnServerStarted += OnServerStarted;
            networkManager.ConnectionApprovalCallback += ApprovalCheck;
            networkManager.OnTransportFailure += OnTransportFailure;
            networkManager.OnServerStopped += OnServerStopped;
        }

        private void OnDestroy()
        {
            if (networkManager != null)
            {
                networkManager.OnClientConnectedCallback -= OnClientConnectedCallback;
                networkManager.OnClientDisconnectCallback -= OnClientDisconnectCallback;
                networkManager.OnServerStarted -= OnServerStarted;
                networkManager.ConnectionApprovalCallback -= ApprovalCheck;
                networkManager.OnTransportFailure -= OnTransportFailure;
                networkManager.OnServerStopped -= OnServerStopped;
            }
        }

        internal void ChangeState(ConnectionState nextState)
        {
            Debug.Log($"{name}: Changed connection state from {currentState.GetType().Name} to {nextState.GetType().Name}.");

            currentState?.Exit();
            currentState = nextState;
            currentState.Enter();
        }

        public void StartClient(string playerName) =>
            currentState.StartClient(playerName);

        public void StartHost(string playerName) =>
            currentState.StartHost(playerName);

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
