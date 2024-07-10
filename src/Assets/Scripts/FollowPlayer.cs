using Unity.Netcode;
using UnityEngine;

public class FollowPlayer : NetworkBehaviour
{
    private Player playerToFollow;
    public float lerpSmoothness = 2f;

    private void Start()
    {
        playerToFollow = transform.parent.GetComponentInChildren<Player>();
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, transform.position.z), lerpSmoothness);
    }

    public void ChangePlayer(Player newPlayer)
    {
        Debug.Log($"Changing player to follow to: {newPlayer.Username.Value}");
        playerToFollow = newPlayer;
    }
}
