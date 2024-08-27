using UnityEngine;

public class EndBlock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.TryGetComponent(out Player finishedPlayer))
        {
            return;
        }

        if (!finishedPlayer.IsLocalPlayer)
        {
            return;
        }

        if (finishedPlayer.NetworkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        finishedPlayer.ReachedEnd();
    }
}
