using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private LayerMask blockLayerMask;

    public bool IsGrounded { get; private set; }

    private void OnCollisionEnter2D(Collision2D collision) =>
        EvalulateCollision(collision);

    private void OnCollisionStay2D(Collision2D collision) =>
        EvalulateCollision(collision);

    private void OnCollisionExit2D(Collision2D collision) =>
        IsGrounded = false;

    private void EvalulateCollision(Collision2D collision)
    {
        for (int contactIdx = 0; contactIdx < collision.contactCount; contactIdx++)
        {
            ContactPoint2D contact = collision.GetContact(contactIdx);
            if (!IsGroundLayer(contact.collider))
            {
                continue;
            }

            IsGrounded |= contact.normal.y >= 0.9f;
        }
    }

    private bool IsGroundLayer(Collider2D collider)
    {
        int colliderLayerMask = 1 << collider.gameObject.layer;
        return (colliderLayerMask & blockLayerMask) != 0;
    }
}
