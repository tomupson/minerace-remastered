using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ClientPlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private float moveHorizontal;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private GroundCheck groundCheck;
    [SerializeField] private NetworkPlayerState networkPlayerState;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float jumpForce;

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

        if (moveHorizontal > 0 && !networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = true;
        }

        if (moveHorizontal < 0 && networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(moveHorizontal * moveSpeed, playerRigidbody.velocity.y);
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            enabled = false;
            return;
        }

        inputReader.OnMoveHook += OnMove;
        inputReader.OnJumpHook += OnJump;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient || !IsOwner)
        {
            return;
        }

        inputReader.OnMoveHook -= OnMove;
        inputReader.OnJumpHook -= OnJump;
    }

    private void OnMove(float moveHorizontal)
    {
        this.moveHorizontal = moveHorizontal;
    }

    public void OnJump()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing || !groundCheck.IsGrounded)
        {
            return;
        }

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
    }
}
