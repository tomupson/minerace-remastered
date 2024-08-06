using Unity.Services.Authentication;
using UnityEngine.SceneManagement;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class OfflineState : ConnectionState
    {
        private const string LobbySceneName = "Lobby";

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

        public override void StartClient(string playerName)
        {
            connectionManager.ChangeState(connectionManager.ClientConnectingState.Configure(playerName));
        }

        public override void StartHost(string playerName)
        {
            connectionManager.ChangeState(connectionManager.StartingHostState.Configure(playerName));
        }
    }
}
