using UnityEngine;

public class Pickaxe : MonoBehaviour
{
    private Player player;
    [SerializeField] private Camera playerCam;

    private void Start()
    {
        player = GetComponentInParent<Player>();
    }

    private void Update()
    {
        GetComponent<SpriteRenderer>().enabled = player.State.Value != PlayerState.Completed;

        if (player.State.Value == PlayerState.InGame && !player.isPaused)
        {
            Vector3 d = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            d.Normalize();

            float rotationZ = Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90);
        }
    }
}
