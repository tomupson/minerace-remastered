using MineRace.Infrastructure;
using MineRace.UGS;
using Unity.Netcode;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class OfflineState : ConnectionState
    {
        private const string MenuSceneName = "Menu";
        private readonly LobbyManager lobbyManager;

        public OfflineState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher, LobbyManager lobbyManager)
            : base(connectionManager, connectStatusPublisher)
        {
            this.lobbyManager = lobbyManager;
        }

        public override async void Enter()
        {
            if (lobbyManager.ActiveLobby != null)
            {
                if (lobbyManager.ActiveLobby.HostId == AuthenticationService.Instance.PlayerId)
                {
                    await lobbyManager.DeleteLobby();
                }
                else
                {
                    await lobbyManager.LeaveLobby();
                }
            }

            NetworkManager.Singleton.Shutdown();
            if (SceneManager.GetActiveScene().name != MenuSceneName)
            {
                SceneManager.LoadScene(MenuSceneName);
            }
        }

        public override void Exit() { }

        public override void StartClient()
        {
            connectionManager.ChangeState(connectionManager.ClientConnectingState);
        }

        public override void StartHost()
        {
            connectionManager.ChangeState(connectionManager.StartingHostState);
        }
    }
}
