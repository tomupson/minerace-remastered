using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public struct NetworkChatMessage : INetworkSerializable
{
    public FixedString64Bytes sender;
    public FixedString512Bytes message;
    public Color colour;

    public NetworkChatMessage(FixedString64Bytes sender, FixedString512Bytes message, Color colour)
    {
        this.sender = sender;
        this.message = message;
        this.colour = colour;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref sender);
        serializer.SerializeValue(ref message);
        serializer.SerializeValue(ref colour);
    }
}
