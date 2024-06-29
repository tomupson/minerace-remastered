using System.Linq;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Handles players reaching the bottom of the map and hitting an "end block"
/// </summary>
public class EndManager : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("PLAYER"))
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player finishedPlayer = collision.transform.GetComponent<Player>();

        // If they are for some reason not in the "in-game" state, return.
        if (finishedPlayer.mode != Player.Mode.InGame)
        {
            return;
        }

        // If that other player has also finished, this means that now both players are done.
        Player otherPlayer = players.FirstOrDefault(z => z != finishedPlayer);
        if (otherPlayer.mode == Player.Mode.Completed)
        {
            FindObjectOfType<GameManager>().GameOverServerRpc();
        }
        else
        {
            if (IsServer)
            {
                finishedPlayer.ReachedEndServerRpc();
            }

            finishedPlayer.mode = Player.Mode.Completed; // Set the completed players mode to completed.
        }

        // Hide player and pickaxe.
        finishedPlayer.GetComponent<SpriteRenderer>().enabled = false;
        finishedPlayer.GetComponentInChildren<Pickaxe>().gameObject.GetComponent<SpriteRenderer>().enabled = false;

        // Disable the box collider to prevent some issues that may arise otherwise.
        finishedPlayer.GetComponent<CircleCollider2D>().enabled = false;
    }
}
