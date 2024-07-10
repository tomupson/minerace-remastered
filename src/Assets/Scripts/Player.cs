using System;
using System.Collections;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    private new Rigidbody2D rigidbody2D;
    private bool canMine = true;
    private float lastSpeed;

    public static Player LocalPlayer { get; private set; }

    public static EventHandler OnAnyPlayerSpawned;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float pickaxeCooldownTime;

    public NetworkVariable<PlayerState> State { get; } = new NetworkVariable<PlayerState>(PlayerState.WaitingForPlayers);

    public NetworkVariable<int> Points { get; } = new NetworkVariable<int>();

    public NetworkVariable<FixedString64Bytes> Username { get; } = new NetworkVariable<FixedString64Bytes>(writePerm: NetworkVariableWritePermission.Owner);

    public bool isPaused;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
        {
            return;
        }

        if (State.Value == PlayerState.InGame && !isPaused)
        {
            float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed;
            if (horizontalSpeed != lastSpeed)
            {
                Vector3 scale = transform.localScale;
                scale.x = horizontalSpeed >= 0 ? 1 : -1;
                transform.localScale = scale;
                lastSpeed = horizontalSpeed;
            }

            rigidbody2D.velocity = new Vector2(horizontalSpeed, 0);
        }
    }

    private void Update()
    {
        if (!IsOwner)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            ChatManager.Instance.ChatSendMessage(Username.Value.ToString(), "Hey I'm good");
        }

        if (State.Value == PlayerState.InGame && !isPaused)
        {
            // Fetch my camera (there are two cameras but because we are fetching one it's the first one)
            Camera playerCam = GetComponentInChildren<Camera>();
            Vector3 direction = playerCam.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            direction.z = 0;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.75f);
            Debug.DrawRay(transform.position, direction, Color.red);
            if (hit)
            {
                Block[] blocksInMap = FindObjectsOfType<Block>();
                for (int i = 0; i < blocksInMap.Length; i++)
                {
                    SpriteRenderer spriteRenderer = blocksInMap[i].gameObject.GetComponent<SpriteRenderer>();
                    spriteRenderer.sprite = blocksInMap[i].blockTextures[0];
                }

                if (hit.transform.CompareTag("BORDER"))
                {
                    return;
                }

                Block b = hit.transform.GetComponent<Block>();
                SpriteRenderer blockRenderer = hit.transform.GetComponent<SpriteRenderer>();
                blockRenderer.sprite = b.blockOutlineTextures[b.textureIndex];

                if (Input.GetButtonDown("BreakBlock") && canMine)
                {
                    AudioManager.Instance.PlaySound(b.blockBreakSoundName);
                    hit.transform.gameObject.SetActive(false); // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you cant mine it twice.
                    BreakBlockServerRpc(hit.transform.gameObject);
                    StartCoroutine(PickaxeCooldown());
                }
            }
            else
            {
                Block[] blocksInMap = FindObjectsOfType<Block>();
                for (int i = 0; i < blocksInMap.Length; i++)
                {
                    SpriteRenderer blockRenderer = blocksInMap[i].gameObject.GetComponent<SpriteRenderer>();
                    blockRenderer.sprite = blocksInMap[i].blockTextures[0];
                }
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            LocalPlayer = this;
            Username.Value = UserAccountManager.Instance.UserInfo.Username;
        }

        OnAnyPlayerSpawned?.Invoke(this, EventArgs.Empty);
    }

    [ServerRpc]
    private void BreakBlockServerRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            Points.Value += networkObject.GetComponent<Block>().blockPointsValue;
        }

        BreakBlockClientRpc(reference);
    }

    [ClientRpc]
    private void BreakBlockClientRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            Destroy(networkObject);
        }
    }

    [ServerRpc]
    public void ReachedEndServerRpc()
    {
        int timeRemainingSeconds = GameManager.Instance.TimeRemaining.Value;
        Points.Value += Mathf.FloorToInt(timeRemainingSeconds / 4f);
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetModeServerRpc(PlayerState state)
    {
        State.Value = state;
    }

    private IEnumerator PickaxeCooldown()
    {
        canMine = false;
        yield return new WaitForSecondsRealtime(pickaxeCooldownTime);
        canMine = true;
    }
}
