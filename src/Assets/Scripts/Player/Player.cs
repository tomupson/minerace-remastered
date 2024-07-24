using System;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private PlayerInputActions inputActions;
    private bool canMine = true;
    private float pickaxeCooldownTimer;
    private RaycastHit2D lastRaycastHit;

    public static Player LocalPlayer { get; private set; }

    public static event Action<Player> OnAnyPlayerSpawned;

    public event Action<Player> OnSpectating;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pickaxeCooldownTime;

    public NetworkVariable<PlayerState> State { get; } = new NetworkVariable<PlayerState>(PlayerState.WaitingForPlayers);

    public NetworkVariable<int> Points { get; } = new NetworkVariable<int>();

    public NetworkVariable<FixedString64Bytes> Username { get; } = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> FacingRight { get; } = new NetworkVariable<bool>(true, writePerm: NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.Mine.performed += OnMinePerformed;
        inputActions.Player.SendChat.performed += OnSendChatPerformed;
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        if (State.Value != PlayerState.Playing || PauseManager.Instance.IsPaused)
        {
            return;
        }

        float horizontal = inputActions.Player.Move.ReadValue<float>();
        float horizontalSpeed = horizontal * moveSpeed;
        if (horizontalSpeed > 0 && !FacingRight.Value)
        {
            FacingRight.Value = true;
        }

        if (horizontalSpeed < 0 && FacingRight.Value)
        {
            FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(horizontalSpeed, 0);
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (State.Value != PlayerState.Playing)
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

        if (PauseManager.Instance.IsPaused)
        {
            return;
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

    public override void OnDestroy()
    {
        inputActions.Player.Mine.performed -= OnMinePerformed;
        inputActions.Player.SendChat.performed -= OnSendChatPerformed;
        inputActions.Dispose();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalPlayer = this;
            Username.Value = UserAccountManager.Instance.UserInfo.Username;
            GameManager.Instance.State.OnValueChanged += OnGameStateChanged;

            transform.position = new Vector3(LevelGenerator.Instance.mapWidth / 2 + 12 * ((int)OwnerClientId * 2 - 1), 100, 0);
        }

        OnAnyPlayerSpawned?.Invoke(this);
        State.OnValueChanged += OnPlayerStateChanged;
        FacingRight.OnValueChanged += OnFacingRightChanged;
    }

    public void ReachedEnd()
    {
        ReachedEndServerRpc();
        SetModeServerRpc(PlayerState.Completed);
    }

    public void Spectate(Player player)
    {
        Camera.main.GetComponent<FollowPlayer>().SwitchTo(player);
        SetModeServerRpc(PlayerState.Spectating);
        OnSpectating?.Invoke(player);
    }

    public void Ready()
    {
        SetModeServerRpc(PlayerState.Ready);
    }

    [ServerRpc]
    private void SetModeServerRpc(PlayerState state)
    {
        // If the client wants to move states backwards, ignore the RPC
        if (State.Value >= state)
        {
            return;
        }

        State.Value = state;
    }

    [ServerRpc]
    private void BreakBlockServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.Despawn();

            Block block = networkObject.GetComponent<BlockRenderer>().block;
            Points.Value += block.pointsValue;
        }
    }

    [ServerRpc]
    private void ReachedEndServerRpc()
    {
        int timeRemainingSeconds = GameManager.Instance.TimeRemaining.Value;
        Points.Value += Mathf.FloorToInt(timeRemainingSeconds / 4f);
    }

    private void OnMinePerformed(InputAction.CallbackContext context)
    {
        if (!canMine)
        {
            return;
        }

        RaycastHit2D hit = PerformMouseRaycast();
        if (!hit || hit.transform.CompareTag("BORDER"))
        {
            return;
        }

        Block hitBlock = hit.transform.GetComponent<BlockRenderer>().block;

        AudioManager.Instance.PlayOneShot(hitBlock.breakSound, hit.transform.position);

        // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you can't mine it twice
        hit.transform.gameObject.SetActive(false);

        canMine = false;

        BreakBlockServerRpc(hit.transform.gameObject);
    }

    private void OnSendChatPerformed(InputAction.CallbackContext context)
    {
        ChatManager.Instance.SendMessage(Username.Value.ToString(), "This is a chat message");
    }

    // TODO: Think about whether the player should update it's own state based on the movement of the game (current behaviour),
    // or if the game manager has authority to change the players' states (would require "RequireOwnership = false" on the ServerRpc)
    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.InGame && State.Value != PlayerState.Playing)
        {
            SetModeServerRpc(PlayerState.Playing);
        }

        if (newState == GameState.Completed && State.Value != PlayerState.Completed)
        {
            SetModeServerRpc(PlayerState.Completed);
        }
    }

    private void OnPlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Completed)
        {
            spriteRenderer.enabled = false;
            circleCollider.enabled = false;
        }
    }

    private void OnFacingRightChanged(bool previousFacingRight, bool newFacingRight)
    {
        spriteRenderer.flipX = !newFacingRight;
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
