using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
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

        if (moveHorizontal > 0 && !networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = true;
        }

        if (moveHorizontal < 0 && networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(moveHorizontal * moveSpeed, 0);
    }

    public void SetMoveInput(float moveHorizontal)
    {
        this.moveHorizontal = moveHorizontal;
    }
}
