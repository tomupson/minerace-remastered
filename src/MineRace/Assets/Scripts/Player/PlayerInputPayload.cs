using Unity.Netcode;

public struct PlayerInputPayload : INetworkSerializable
{
    public int tick;
    public float moveHorizontal;

    public PlayerInputPayload(int tick, float moveHorizontal)
    {
        this.tick = tick;
        this.moveHorizontal = moveHorizontal;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref tick);
        serializer.SerializeValue(ref moveHorizontal);
    }
}
