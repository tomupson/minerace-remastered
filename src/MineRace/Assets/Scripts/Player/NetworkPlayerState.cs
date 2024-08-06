using Unity.Collections;
using Unity.Netcode;

public class NetworkPlayerState : NetworkBehaviour
{
    public NetworkVariable<PlayerState> State { get; } = new NetworkVariable<PlayerState>(PlayerState.WaitingForPlayers);

    public NetworkVariable<int> Points { get; } = new NetworkVariable<int>();

    public NetworkVariable<FixedString64Bytes> Username { get; } = new NetworkVariable<FixedString64Bytes>();

    public NetworkVariable<bool> FacingRight { get; } = new NetworkVariable<bool>(true, writePerm: NetworkVariableWritePermission.Owner);
}
