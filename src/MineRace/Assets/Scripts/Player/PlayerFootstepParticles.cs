using UnityEngine;

public class PlayerFootstepParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem footstepsParticleSystem;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // TODO: Ensure the block we're checking is below us

        if (!collision.gameObject.TryGetComponent(out BlockRenderer blockRenderer))
        {
            return;
        }

        ParticleSystem.MainModule mainModule = footstepsParticleSystem.main;
        mainModule.startColor = new ParticleSystem.MinMaxGradient(blockRenderer.block.primaryColour, blockRenderer.block.secondaryColour);
    }
}
