using Unity.Collections;
using Unity.Netcode;

namespace MineRace.ConnectionManagement
{
    public readonly struct ConnectionEventMessage : INetworkSerializeByMemcpy
    {
        public ConnectStatus ConnectStatus { get; }
        public FixedString64Bytes PlayerName { get; }

        public ConnectionEventMessage(ConnectStatus connectStatus, FixedString64Bytes playerName)
        {
            ConnectStatus = connectStatus;
            PlayerName = playerName;
        }
    }
}
