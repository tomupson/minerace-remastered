using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;

namespace MineRace.ConnectionManagement.States
{
    internal sealed class ClientConnectedState : OnlineState
    {
        public ClientConnectedState(ConnectionManager connectionManager, IPublisher<ConnectStatus> connectStatusPublisher)
            : base(connectionManager, connectStatusPublisher)
        {
        }

        public override void Enter() { }

        public override void Exit() { }

        public override void OnClientDisconnect(ulong clientId)
        {
            string disconnectReason = NetworkManager.Singleton.DisconnectReason;
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
