using Unity.Collections;
using Unity.Netcode;

namespace MineRace.ConnectionManagement
{
    public struct NetworkConnectionEventMessage : INetworkSerializable
    {
        public ulong clientId;
        public ConnectStatus connectStatus;
        public FixedString64Bytes playerName;

        public NetworkConnectionEventMessage(ulong clientId, ConnectStatus connectStatus, FixedString64Bytes playerName)
        {
            this.clientId = clientId;
            this.connectStatus = connectStatus;
            this.playerName = playerName;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref clientId);
            serializer.SerializeValue(ref connectStatus);
            serializer.SerializeValue(ref playerName);
        }
    }
}
