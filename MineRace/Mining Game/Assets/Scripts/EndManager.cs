using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

/// <summary>
/// EndManager.cs handles players reaching the bottom of the map and hitting an "end block".
/// </summary>

public class EndManager : NetworkBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) // Tests if an object collides with the bottom of the map
    {
        if (collision.transform.tag == "PLAYER") // If it's a player...
        {
            Player[] players = FindObjectsOfType<Player>();
            Player finishedPlayer = collision.transform.GetComponent<Player>(); // Get the player that has reached the end
            if (finishedPlayer.mode != Player.Mode.inGame) return; // If they are for some reason not in the "in-game" state, return.
            Player otherPlayer = players.Where(z => z != finishedPlayer).FirstOrDefault(); // Grab the other player that is still in the game.
            if (otherPlayer.mode == Player.Mode.completed) // If that other player has also finished, this means that now both players are done.
            {
                FindObjectOfType<GameManager>().CmdGameOver(); // So we trigger the gameover method in the GameManager.
            } else
            {
                if (isServer)
                    finishedPlayer.CmdReachedEnd(); // Otherwise we just trigger the function for the player reaching the end while the other player is still in-game
                finishedPlayer.mode = Player.Mode.completed; // Set the completed players mode to completed.
            }

            finishedPlayer.GetComponent<SpriteRenderer>().enabled = false; // Disable the sprite renderer so we cant see the player model anymore
            finishedPlayer.GetComponentInChildren<Pickaxe>().gameObject.GetComponent<SpriteRenderer>().enabled = false; // Disable the pickaxe's sprite renderer so we cant see the pickaxe anymore.
            finishedPlayer.GetComponent<CircleCollider2D>().enabled = false; // Disable the box collider to prevent some issues that may arise otherwise.
        }
    }
}
