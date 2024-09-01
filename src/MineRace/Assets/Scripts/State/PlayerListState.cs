using System;
using Unity.Collections;
using Unity.Netcode;

public struct PlayerListState : INetworkSerializable, IEquatable<PlayerListState>
{
    public ulong clientId;
    public FixedString64Bytes playerName;

    public PlayerListState(ulong clientId, FixedString64Bytes playerName)
    {
        this.clientId = clientId;
        this.playerName = playerName;
    }

    public readonly bool Equals(PlayerListState other)
    {
        return clientId == other.clientId
            && playerName == other.playerName;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
    }
}
