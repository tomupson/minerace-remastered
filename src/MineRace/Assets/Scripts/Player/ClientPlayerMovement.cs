using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    private Rigidbody2D playerRigidbody;
    private float moveHorizontal;

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

    public void SetMoveInput(float moveHorizontal)
    {
        this.moveHorizontal = moveHorizontal;
    }

    public void Jump()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing || !groundCheck.IsGrounded)
        {
            return;
        }

        playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, jumpForce);
    }
}
