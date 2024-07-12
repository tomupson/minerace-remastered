using System.Linq;
using UnityEngine;

/// <summary>
/// Handles players reaching the bottom of the map and hitting an "end block"
/// </summary>
public class EndBlock : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("PLAYER"))
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player finishedPlayer = collision.transform.GetComponent<Player>();

        // This end block instance should only handle collisions for the local player
        if (!finishedPlayer.IsLocalPlayer)
        {
            return;
        }

        // If they are for some reason not in the "playing" state, return
        if (finishedPlayer.State.Value != PlayerState.Playing)
        {
            return;
        }

        // If that other player has also finished, this means that now both players are done
        Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);
        if (otherPlayer.State.Value == PlayerState.Completed)
        {
            // TODO: This logic should be run on the server, it should not be allowed that clients' end blocks drive the game state
            GameManager.Instance.GameOverServerRpc();
        }
        else
        {
            finishedPlayer.ReachedEnd();
        }
    }
}
