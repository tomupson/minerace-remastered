using MineRace.Audio;
using MineRace.Utils.Timers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player), typeof(PlayerMovement), typeof(Rigidbody2D))]
public class ClientPlayerInput : NetworkBehaviour
{
    [SerializeField] private PlayerInputReader inputReader;

    private Player player;
    private PlayerMovement playerMovement;
    private CountdownTimer pickaxeCooldownTimer;
    private RaycastHit2D lastRaycastHit;

    [SerializeField] private NetworkPlayerState networkPlayerState;
    [SerializeField] private float pickaxeCooldownTime;
    [SerializeField] private LayerMask blockLayerMask;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        pickaxeCooldownTimer = new CountdownTimer(pickaxeCooldownTime);
    }

    private void Update()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        pickaxeCooldownTimer.Tick(Time.deltaTime);

        RaycastHit2D hit = PerformMouseRaycast();

        if (lastRaycastHit && (!hit || lastRaycastHit.transform != hit.transform))
        {
            Block lastHitBlock = lastRaycastHit.transform.GetComponent<BlockRenderer>().block;
            SpriteRenderer lastHitBlockSpriteRenderer = lastRaycastHit.transform.GetComponent<SpriteRenderer>();
            lastHitBlockSpriteRenderer.sprite = lastHitBlock.textures[lastHitBlock.textureIndex];
        }

        if (hit)
        {
            lastRaycastHit = hit;

            Block hitBlock = hit.transform.GetComponent<BlockRenderer>().block;
            SpriteRenderer hitBlockSpriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
            hitBlockSpriteRenderer.sprite = hitBlock.outlineTextures[hitBlock.textureIndex];
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsClient || !IsOwner)
        {
            enabled = false;
            return;
        }

        inputReader.OnMineHook += OnMine;
        inputReader.OnMoveHook += OnMove;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsClient || !IsOwner)
        {
            return;
        }

        inputReader.OnMineHook -= OnMine;
        inputReader.OnMoveHook -= OnMove;
    }

    private void OnMove(float horizontal)
    {
        playerMovement.SetMoveInput(horizontal);
    }

    private void OnMine()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing || pickaxeCooldownTimer.IsRunning)
        {
            return;
        }

        RaycastHit2D hit = PerformMouseRaycast();
        if (!hit)
        {
            return;
        }

        Block hitBlock = hit.transform.GetComponent<BlockRenderer>().block;

        if (hitBlock.breakSound != null)
        {
            AudioManager.PlayOneShot(hitBlock.breakSound, hit.transform.position);
        }

        // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you can't mine it twice
        hit.transform.gameObject.SetActive(false);

        pickaxeCooldownTimer.Start();

        player.BreakBlock(hit.transform.gameObject);
    }

    private RaycastHit2D PerformMouseRaycast()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        direction.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.75f, blockLayerMask);
        Debug.DrawRay(transform.position, direction, Color.red);
        return hit;
    }
}
