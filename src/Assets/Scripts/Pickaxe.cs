using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    private Player player;
    [SerializeField] private Camera playerCam;

    void Start()
    {
        player = GetComponentInParent<Player>();
    }

    void Update()
    {
        GetComponent<SpriteRenderer>().enabled = player.mode != Player.Mode.Completed;

        if (player.mode == Player.Mode.InGame && !player.isPaused)
        {
            Vector3 d = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            d.Normalize();

            float rotationZ = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90);
        }
    }
}
