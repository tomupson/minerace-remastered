using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Player playerToFollow;
    public float lerpSmoothness = 2f;

    void Start()
    {
        playerToFollow = transform.parent.GetComponentInChildren<Player>();
    }

    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, transform.position.z), lerpSmoothness);
    }

    public void ChangePlayer(Player np)
    {
        Debug.Log("Changing player to follow to: " + np.username);
        playerToFollow = np;
    }

}
