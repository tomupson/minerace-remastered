using MineRace.UGS;
using Unity.Services.Authentication;
using UnityEngine.SceneManagement;
using VContainer;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class OfflineState : ConnectionState
    {
        private const string LobbySceneName = "Lobby";

        [Inject] private readonly LobbyManager lobbyManager;

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

            networkManager.Shutdown();
            if (SceneManager.GetActiveScene().name != LobbySceneName)
            {
                SceneManager.LoadScene(LobbySceneName);
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
