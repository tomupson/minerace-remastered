using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Player playerToFollow;

    [SerializeField] private float lerpSmoothness = 2f;
    [SerializeField] private PlayerGameEvent localPlayerSpawnedEvent;

    private void Awake()
    {
        localPlayerSpawnedEvent.RegisterListener(OnLocalPlayerSpawned);
    }

    private void OnDestroy()
    {
        localPlayerSpawnedEvent.DeregisterListener(OnLocalPlayerSpawned);
    }

    private void LateUpdate()
    {
        if (playerToFollow == null)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, transform.position.z), lerpSmoothness);
    }

    public void SwitchTo(Player player)
    {
        playerToFollow = player;
    }

    private void OnLocalPlayerSpawned(Player player)
    {
        if (playerToFollow == null)
        {
            playerToFollow = player;
        }
    }
}
