using UnityEngine;

public class PlayerFootstepParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem footstepsParticleSystem;
    [SerializeField] private LayerMask blockLayerMask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        for (int contactIdx = 0; contactIdx < collision.contactCount; contactIdx++)
        {
            ContactPoint2D contact = collision.GetContact(contactIdx);
            if (!IsGroundLayer(contact.collider) || contact.normal.y < 0.9f)
            {
                continue;
            }

            if (!contact.collider.gameObject.TryGetComponent(out BlockRenderer blockRenderer))
            {
                return;
            }

            ParticleSystem.MainModule mainModule = footstepsParticleSystem.main;
            mainModule.startColor = new ParticleSystem.MinMaxGradient(blockRenderer.block.primaryColour, blockRenderer.block.secondaryColour);
            break;
        }

    }

    private bool IsGroundLayer(Collider2D collider)
    {
        int colliderLayerMask = 1 << collider.gameObject.layer;
        return (colliderLayerMask & blockLayerMask) != 0;
    }
}
