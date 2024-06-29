using System.Collections;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    private Rigidbody2D rb;
    private Player_SyncScale syncScale;
    private bool canMine;
    private bool facingRight = true;

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed; // Player's Speed
    [SerializeField] private float pickaxeCooldownTime; // Player's Pickaxe cooldown time.

    [HideInInspector] public NetworkVariable<int> points;
    [HideInInspector] public NetworkVariable<bool> ready;
    [HideInInspector] public NetworkVariable<string> username;

    public bool isPaused;
    public Mode mode;

    public enum Mode
    {
        WaitingForPlayers, // When waiting for players to join the game.
        ReadyUp, // When the "Ready" button is enabled.
        WaitingForPlayerReady, // When you're waiting for the other player to ready up.
        PregameCountdown, // When the 10 second countdown is counting down.
        InGame, // When playing the game.
        Completed, // When you reach the end of the game or the time runs out.
        Spectating, // When you're spectating another player.
        GameOver // When the game is finished for all players.
    }

    void Awake()
    {
        syncScale = GetComponent<Player_SyncScale>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true;

        isPaused = false;

        SetUsernameServerRpc(UserAccountManager.Instance.userInfo.Username);

        mode = Mode.WaitingForPlayers;
        canMine = true;
    }

    [ServerRpc]
    void SetUsernameServerRpc(string username)
    {
        username = !string.IsNullOrEmpty(username) ? username : "Guest";
        this.username.Value = username;
    }

    void FixedUpdate()
    {
        if (mode == Mode.InGame && !isPaused)
        {
            float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed;

            if (horizontalSpeed > 0 && !facingRight || horizontalSpeed < 0 && facingRight)
            {
                syncScale.FlipSpriteServerRpc(facingRight);
            }

            rb.velocity = new Vector2(horizontalSpeed, 0);
        }
    }

    /*[ServerRpc]
    void SetFlipStateServerRpc()
    {
        facingRight = !facingRight; // Change your direction
        ChangeFlipStateOnClientsClientRpc();
    }

    [ClientRpc]
    void ChangeFlipStateOnClientsClientRpc()
    {
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(s.x * -1, s.y, s.z); // Flip the player when the direction has been switched.
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChatManager.Instance.ChatSendMessage(username.Value, "Hey I'm good");
        }

        if (mode == Mode.InGame && !isPaused)
        {
            Camera playerCam = GetComponentInChildren<Camera>(); // Fetch My Camera (there are two cameras but because we are fetching one it's the first one).
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

                if (hit.transform.tag == "BORDER")
                    return;

                Block b = hit.transform.GetComponent<Block>();
                SpriteRenderer blockRenderer = hit.transform.GetComponent<SpriteRenderer>();
                blockRenderer.sprite = b.blockOutlineTextures[b.textureIndex];

                if (Input.GetButtonDown("BreakBlock") && canMine)
                {
                    AudioManager.Instance.PlaySound(b.blockBreakSoundName);
                    hit.transform.gameObject.SetActive(false); // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you cant mine it twice.
                    BreakBlockServerRpc(hit.transform.gameObject.GetComponent<Block>());
                    StartCoroutine(PickaxeCooldown());
                }
            } else
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
    
    IEnumerator PickaxeCooldown()
    {
        canMine = false;
        yield return new WaitForSecondsRealtime(pickaxeCooldownTime);
        canMine = true;
    }

    [ServerRpc]
    void BreakBlockServerRpc(NetworkBehaviourReference reference)
    {
        //NetworkServer.Destroy(obj);
        if (reference.TryGet(out Block b))
        {
            AddPointsClientRpc(b.blockPointsValue);
        }
    }

    [ClientRpc]
    void AddPointsClientRpc(int p)
    {
        points.Value += p;
    }

    [ServerRpc]
    public void ReachedEndServerRpc()
    {
        AddPointsClientRpc(Mathf.FloorToInt(FindObjectOfType<GameManager>().timeLeft.Value / 4f));
    }

    [ServerRpc]
    public void ChangeReadyStateServerRpc()
    {
        ready.Value = true;
    }

    void OnDirectionChange(bool facingRight)
    {
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(s.x * -1, s.y, s.z);
    }
}