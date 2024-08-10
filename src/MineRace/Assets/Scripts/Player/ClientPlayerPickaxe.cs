using System;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class ClientPlayerPickaxe : NetworkBehaviour
{
    private DisposableGroup subscriptions;

    private Player player;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (player.NetworkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        Vector3 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        direction.Normalize();

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90);
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsClient && IsOwner;

        subscriptions = new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));
        subscriptions.Add(player.NetworkPlayerState.FacingRight.Subscribe(OnPlayerFacingRightChanged));
    }

    public override void OnNetworkDespawn()
    {
        subscriptions?.Dispose();
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state == PlayerState.Completed)
        {
            spriteRenderer.enabled = false;
        }
    }

    private void OnPlayerFacingRightChanged(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;

        float localPositionX = Math.Abs(transform.localPosition.x);
        localPositionX = facingRight ? localPositionX : -localPositionX;
        transform.localPosition = new Vector3(localPositionX, transform.localPosition.y, transform.localPosition.z);
    }
}
