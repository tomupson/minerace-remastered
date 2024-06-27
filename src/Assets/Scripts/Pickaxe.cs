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
        if (player.mode == Player.Mode.completed)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        } else
        {
            GetComponent<SpriteRenderer>().enabled = true;
        }

        if (player.mode == Player.Mode.inGame && !player.isPaused)
        {
            Vector3 d = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            d.Normalize();
            float rotationZ = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90);
        }
    }
}
