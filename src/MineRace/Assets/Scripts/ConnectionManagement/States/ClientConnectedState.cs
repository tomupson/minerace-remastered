using UnityEngine;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class ClientConnectedState : OnlineState
    {
        public override void Enter() { }

        public override void Exit() { }

        public override void OnClientDisconnect(ulong clientId)
        {
            string disconnectReason = networkManager.DisconnectReason;
            if (string.IsNullOrEmpty(disconnectReason))
            {
                connectStatusPublisher.Publish(ConnectStatus.Reconnecting);
                connectionManager.ChangeState(connectionManager.ClientReconnectingState);
            }
            else
            {
                ConnectStatus connectStatus = JsonUtility.FromJson<ConnectStatus>(disconnectReason);
                connectStatusPublisher.Publish(connectStatus);
                connectionManager.ChangeState(connectionManager.OfflineState);
            }
        }
    }
}
