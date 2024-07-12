using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;
    private bool canMine = true;

    public static Player LocalPlayer { get; private set; }

    public static event EventHandler OnAnyPlayerSpawned;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pickaxeCooldownTime;

    public NetworkVariable<PlayerState> State { get; } = new NetworkVariable<PlayerState>(PlayerState.WaitingForPlayers);

    public NetworkVariable<int> Points { get; } = new NetworkVariable<int>();

    public NetworkVariable<FixedString64Bytes> Username { get; } = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);

    public NetworkVariable<bool> FacingRight { get; } = new NetworkVariable<bool>(true, writePerm: NetworkVariableWritePermission.Owner);

    [HideInInspector] public bool isPaused;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        if (State.Value != PlayerState.Playing || isPaused)
        {
            return;
        }

        float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed;
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

        if (Input.GetKeyDown(KeyCode.M))
        {
            ChatManager.Instance.SendMessage(Username.Value.ToString(), "This is a chat message");
        }

        if (State.Value != PlayerState.Playing || isPaused)
        {
            return;
        }

        Vector3 direction = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        direction.z = 0;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.75f);
        Debug.DrawRay(transform.position, direction, Color.red);

        if (hit)
        {
            // TODO: Refactor this - do we really need to look at every block on the map to reset it's block texture?
            Block[] blocksInMap = FindObjectsOfType<Block>();
            for (int i = 0; i < blocksInMap.Length; i++)
            {
                SpriteRenderer spriteRenderer = blocksInMap[i].gameObject.GetComponent<SpriteRenderer>();
                spriteRenderer.sprite = blocksInMap[i].blockTextures[blocksInMap[i].textureIndex];
            }

            if (hit.transform.CompareTag("BORDER"))
            {
                return;
            }

            Block hitBlock = hit.transform.GetComponent<Block>();
            SpriteRenderer hitBlockSpriteRenderer = hit.transform.GetComponent<SpriteRenderer>();
            hitBlockSpriteRenderer.sprite = hitBlock.blockOutlineTextures[hitBlock.textureIndex];

            if (Input.GetButtonDown("BreakBlock") && canMine)
            {
                AudioManager.Instance.PlaySound(hitBlock.blockBreakSoundName);

                // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you cant mine it twice
                hit.transform.gameObject.SetActive(false);

                BreakBlockServerRpc(hit.transform.gameObject);
                StartCoroutine(PickaxeCooldown());
            }

            hitBlockSpriteRenderer.sprite = hitBlock.blockTextures[hitBlock.textureIndex];
        }
        else
        {
            // TODO: Refactor this - do we really need to look at every block on the map to reset it's block texture?
            Block[] blocksInMap = FindObjectsOfType<Block>();
            for (int i = 0; i < blocksInMap.Length; i++)
            {
                SpriteRenderer blockRenderer = blocksInMap[i].gameObject.GetComponent<SpriteRenderer>();
                blockRenderer.sprite = blocksInMap[i].blockTextures[blocksInMap[i].textureIndex];
            }
        }
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

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
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
    }

    public void Ready()
    {
        SetModeServerRpc(PlayerState.Ready);
    }

    [ServerRpc]
    private void SetModeServerRpc(PlayerState state)
    {
        // TODO: Validate the state transition is valid?
        State.Value = state;
    }

    [ServerRpc]
    private void BreakBlockServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.Despawn();

            Block block = networkObject.GetComponent<Block>();
            Points.Value += block.blockPointsValue;
        }
    }

    [ServerRpc]
    private void ReachedEndServerRpc()
    {
        int timeRemainingSeconds = GameManager.Instance.TimeRemaining.Value;
        Points.Value += Mathf.FloorToInt(timeRemainingSeconds / 4f);
    }

    private IEnumerator PickaxeCooldown()
    {
        canMine = false;
        yield return new WaitForSecondsRealtime(pickaxeCooldownTime);
        canMine = true;
    }

    // TODO: Think about whether the player should update it's own state based on the movement of the game,
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
}
