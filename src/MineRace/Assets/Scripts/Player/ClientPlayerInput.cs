using MineRace.Audio;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

[RequireComponent(typeof(Player), typeof(Rigidbody2D))]
public class ClientPlayerInput : NetworkBehaviour
{
    [Inject] private readonly PlayerInputReader inputReader;

    private Player player;
    private Rigidbody2D playerRigidbody;

    private bool canMine = true;
    private float pickaxeCooldownTimer;
    private RaycastHit2D lastRaycastHit;

    [SerializeField] private NetworkPlayerState networkPlayerState;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pickaxeCooldownTime;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerRigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        if (!canMine)
        {
            pickaxeCooldownTimer -= Time.deltaTime;
            if (pickaxeCooldownTimer <= 0f)
            {
                canMine = true;
                pickaxeCooldownTimer = pickaxeCooldownTime;
            }
        }

        RaycastHit2D hit = PerformMouseRaycast();

        if (lastRaycastHit && (!hit || lastRaycastHit.transform != hit.transform))
        {
            Block lastHitBlock = lastRaycastHit.transform.GetComponent<BlockRenderer>().block;
            SpriteRenderer lastHitBlockSpriteRenderer = lastRaycastHit.transform.GetComponent<SpriteRenderer>();
            lastHitBlockSpriteRenderer.sprite = lastHitBlock.textures[lastHitBlock.textureIndex];
        }

        lastRaycastHit = hit;

        if (hit && !hit.transform.CompareTag("BORDER"))
        {
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
    }

    private void OnMove(float horizontal)
    {
        if (networkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        float horizontalSpeed = horizontal * moveSpeed;
        if (horizontalSpeed > 0 && !networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = true;
        }

        if (horizontalSpeed < 0 && networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(horizontalSpeed, 0);
    }

    private void OnMine()
    {
        if (networkPlayerState.State.Value != PlayerState.Playing || !canMine)
        {
            return;
        }

        RaycastHit2D hit = PerformMouseRaycast();
        if (!hit || hit.transform.CompareTag("BORDER"))
        {
            return;
        }

        Block hitBlock = hit.transform.GetComponent<BlockRenderer>().block;

        AudioManager.PlayOneShot(hitBlock.breakSound, hit.transform.position);

        // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you can't mine it twice
        hit.transform.gameObject.SetActive(false);

        canMine = false;

        player.BreakBlockServerRpc(hit.transform.gameObject);
    }

    private RaycastHit2D PerformMouseRaycast()
    {
        Vector3 direction = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position;
        direction.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.75f);
        Debug.DrawRay(transform.position, direction, Color.red);
        return hit;
    }
}
