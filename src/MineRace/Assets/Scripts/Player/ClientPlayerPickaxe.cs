using System;
using MineRace.Audio;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using MineRace.Utils.Timers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SpriteRenderer))]
public class ClientPlayerPickaxe : NetworkBehaviour
{
    private DisposableGroup subscriptions;
    private Player player;
    private SpriteRenderer spriteRenderer;
    private CountdownTimer pickaxeCooldownTimer;
    private RaycastHit2D lastRaycastHit;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private float reach = 0.75f;
    [SerializeField] private float pickaxeCooldownTime;
    [SerializeField] private LayerMask blockLayerMask;

    private void Awake()
    {
        player = GetComponentInParent<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        pickaxeCooldownTimer = new CountdownTimer(pickaxeCooldownTime);
    }

    private void Update()
    {
        if (player.NetworkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        pickaxeCooldownTimer.Tick(Time.deltaTime);

        Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector3 direction = targetPosition - transform.position;
        direction.z = 0;

        float rotationZ = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotationZ + 90f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, reach, blockLayerMask);
        Debug.DrawRay(transform.position, direction, Color.red);

        if (lastRaycastHit && (!hit || lastRaycastHit.transform != hit.transform))
        {
            ClearBlockOutline(lastRaycastHit);
        }

        lastRaycastHit = hit;

        if (hit)
        {
            Block hitBlock = hit.transform.GetComponent<BlockRenderer>().block;
            SpriteRenderer hitBlockSpriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
            hitBlockSpriteRenderer.sprite = hitBlock.outlineTextures[hitBlock.textureIndex];
        }
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsClient && IsOwner;
        if (enabled)
        {
            inputReader.OnMineHook += OnMine;
        }

        subscriptions = new DisposableGroup();
        subscriptions.Add(player.NetworkPlayerState.State.Subscribe(OnPlayerStateChanged));
        subscriptions.Add(player.NetworkPlayerState.FacingRight.Subscribe(OnPlayerFacingRightChanged));
    }

    public override void OnNetworkDespawn()
    {
        subscriptions?.Dispose();
        if (IsClient && IsOwner)
        {
            inputReader.OnMineHook -= OnMine;
        }
    }

    private void OnMine()
    {
        if (player.NetworkPlayerState.State.Value != PlayerState.Playing || pickaxeCooldownTimer.IsRunning)
        {
            return;
        }

        if (!lastRaycastHit)
        {
            return;
        }

        if (lastRaycastHit.point.x < transform.position.x)
        {
            player.NetworkPlayerState.FacingRight.Value = false;
        }
        else if (lastRaycastHit.point.x > transform.position.x)
        {
            player.NetworkPlayerState.FacingRight.Value = true;
        }

        Transform hitTransform = lastRaycastHit.transform;
        Block hitBlock = hitTransform.GetComponent<BlockRenderer>().block;

        if (hitBlock.breakSound != null)
        {
            AudioManager.PlayOneShot(hitBlock.breakSound, hitTransform.position);
        }

        // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you can't mine it twice
        hitTransform.gameObject.SetActive(false);

        pickaxeCooldownTimer.Start();

        player.BreakBlock(hitTransform.gameObject);
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state == PlayerState.Completed)
        {
            spriteRenderer.enabled = false;
            if (lastRaycastHit)
            {
                ClearBlockOutline(lastRaycastHit);
                lastRaycastHit = default;
            }
        }
    }

    private void ClearBlockOutline(RaycastHit2D hit)
    {
        Block lastHitBlock = hit.transform.GetComponent<BlockRenderer>().block;
        SpriteRenderer lastHitBlockSpriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
        lastHitBlockSpriteRenderer.sprite = lastHitBlock.textures[lastHitBlock.textureIndex];
    }

    private void OnPlayerFacingRightChanged(bool facingRight)
    {
        spriteRenderer.flipX = !facingRight;

        float localPositionX = Math.Abs(transform.localPosition.x);
        localPositionX = facingRight ? localPositionX : -localPositionX;
        transform.localPosition = new Vector3(localPositionX, transform.localPosition.y, transform.localPosition.z);
    }
}
