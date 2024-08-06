using UnityEngine;

public class EndBlock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("PLAYER"))
        {
            return;
        }

        Player finishedPlayer = collision.transform.GetComponent<Player>();
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
