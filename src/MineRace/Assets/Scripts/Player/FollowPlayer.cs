using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    private Player playerToFollow;

    [SerializeField] private float lerpSmoothness = 2f;
    [SerializeField] private float distanceZ = 10f;

    private void Awake()
    {
        Player.OnAnyPlayerSpawned += OnAnyPlayedSpawned;
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawned -= OnAnyPlayedSpawned;
    }

    private void Update()
    {
        if (playerToFollow == null)
        {
            return;
        }

        transform.position = Vector3.Lerp(transform.position, new Vector3(playerToFollow.transform.position.x, playerToFollow.transform.position.y, -distanceZ), lerpSmoothness);
    }

    public void SwitchTo(Player player)
    {
        playerToFollow = player;
    }

    private void OnAnyPlayedSpawned(Player player)
    {
        if (playerToFollow == null)
        {
            playerToFollow = Player.LocalPlayer;
        }
    }
}
