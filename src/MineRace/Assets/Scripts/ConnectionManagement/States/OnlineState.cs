using System.Text;
using Unity.Services.Authentication;
using UnityEngine;

namespace MineRace.ConnectionManagement.States
{
    internal abstract class OnlineState : ConnectionState
    {
        public override void OnUserRequestedShutdown()
        {
            connectStatusPublisher.Publish(ConnectStatus.UserRequestedDisconnect);
            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        public override void OnTransportFailure()
        {
            connectionManager.ChangeState(connectionManager.OfflineState);
        }

        protected void SetConnectionPayload(string playerName)
        {
            string payload = JsonUtility.ToJson(new ConnectionPayload(AuthenticationService.Instance.PlayerId, playerName));
            networkManager.NetworkConfig.ConnectionData = Encoding.UTF8.GetBytes(payload);
        }
    }
}
