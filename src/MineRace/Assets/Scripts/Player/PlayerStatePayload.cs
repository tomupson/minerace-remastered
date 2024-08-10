using Unity.Netcode;
using UnityEngine;

public struct PlayerStatePayload : INetworkSerializable
{
    public int tick;
    public Vector2 position;
    public Vector2 velocity;

    public PlayerStatePayload(int tick, Vector2 position, Vector2 velocity)
    {
        this.tick = tick;
        this.position = position;
        this.velocity = velocity;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref position);
        serializer.SerializeValue(ref velocity);
    }
}
