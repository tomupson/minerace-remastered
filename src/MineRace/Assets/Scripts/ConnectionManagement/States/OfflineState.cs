using UnityEngine.SceneManagement;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class OfflineState : ConnectionState
    {
        private const string LobbySceneName = "Lobby";

        public override void Enter()
        {
            lobbyManager.EndTracking();
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
