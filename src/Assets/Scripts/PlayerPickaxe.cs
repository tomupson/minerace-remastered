using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPickaxe : NetworkBehaviour
{
    private Player player;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        player.State.OnValueChanged += OnPlayerStateChanged;
        player.FacingRight.OnValueChanged += OnPlayerFacingRightChanged;
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (player.State.Value != PlayerState.Playing || player.isPaused)
        {
            return;
        }

        Vector3 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        direction.Normalize();

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90);
    }

    private void OnPlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Completed)
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnPlayerFacingRightChanged(bool previousFacingRight, bool newFacingRight)
    {
        spriteRenderer.flipX = !newFacingRight;
        transform.localPosition = new Vector3(transform.localPosition.x * -1, transform.localPosition.y, transform.localPosition.z);
    }
}
