using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ClientPlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private float moveHorizontal;
    private bool jumping;

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

        float yVelocity = groundCheck.IsGrounded && jumping ? jumpForce : playerRigidbody.velocity.y;
        playerRigidbody.velocity = new Vector2(moveHorizontal * moveSpeed, yVelocity);
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
        inputReader.OnJumpCancelledHook += OnJumpCancelled;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient || !IsOwner)
        {
            return;
        }

        inputReader.OnMoveHook -= OnMove;
        inputReader.OnJumpHook -= OnJump;
        inputReader.OnJumpCancelledHook -= OnJumpCancelled;
    }

    private void OnMove(float moveHorizontal) => this.moveHorizontal = moveHorizontal;

    private void OnJump() => jumping = true;

    private void OnJumpCancelled() => jumping = false;
}
