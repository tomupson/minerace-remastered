using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ServerPlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private float moveHorizontal;

    [SerializeField] private NetworkPlayerState networkPlayerState;
    [SerializeField] private float moveSpeed;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        float horizontalSpeed = moveHorizontal * moveSpeed;
        if (horizontalSpeed > 0 && !networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = true;
        }

        if (horizontalSpeed < 0 && networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(horizontalSpeed, 0);
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsServer;
    }

    public void Move(float moveHorizontal)
    {
        MoveServerRpc(moveHorizontal);
    }

    [ServerRpc]
    private void MoveServerRpc(float moveHorizontal)
    {
        // TODO: Validate movement
        this.moveHorizontal = moveHorizontal;
    }
}
