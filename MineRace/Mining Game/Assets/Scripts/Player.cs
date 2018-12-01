using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : NetworkBehaviour
{
    private Rigidbody2D rb; // The players Rigidbody.

    [Header("Player Settings")]
    [SerializeField] private float moveSpeed; // Player's Speed
    [SerializeField] private float pickaxeCooldownTime; // Player's Pickaxe cooldown time.

    private bool facingRight = true; // Whether the player is facing right or not.

    [HideInInspector] [SyncVar] public int points; // How many points the player has.
    [SyncVar] [HideInInspector] public bool ready; // If the player is ready to play.
    private bool canMine; // Determines if the player can mine the block.

    [SyncVar] public string username; // The players username.

    public bool isPaused;

    public Mode mode; // The players in-match mode.

    private Player_SyncScale syncScale;

    public enum Mode
    {
        waitingForPlayers, // When waiting for players to join the game
        readyUp, // When the "Ready" button is enabled.
        waitingForPlayerReady, // When you're waiting for the other player to ready up.
        pregameCountdown, // When the 10 second countdown is counting down.
        inGame, // When playing the game.
        completed, // When you reach the end of the game or the time runs out.
        spectating, // When you're spectating another player.
        gameOver // When the game is finished for all players.
    }

    void Awake()
    {
        syncScale = GetComponent<Player_SyncScale>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        facingRight = true; // Start facing right

        isPaused = false;

        CmdSetUsername(UserAccountManager.instance.userInfo.Username);

        mode = Mode.waitingForPlayers; // Start off waiting for players.
        canMine = true; // You can mine at the start of the game.
    }

    [Command]
    void CmdSetUsername(string _username)
    {
        username = _username; // Set your username to the username of the logged in player from the database.
        if (username == null || username == "")
            username = "Guest"; // Set your username to a guest name if there is no username somehow.
    }

    void FixedUpdate()
    {
        if (mode == Mode.inGame && !isPaused) // If playing...
        {
            float horizontalSpeed = Input.GetAxis("Horizontal") * moveSpeed; // Speed left or right

            if (horizontalSpeed > 0 && !facingRight || horizontalSpeed < 0 && facingRight) // If you were moving left and now have a right velocity
            { // or were moving right and now have a left velocity,
                syncScale.CmdFlipSprite(facingRight);
            }

            rb.velocity = new Vector2(horizontalSpeed, 0); // Move in the direction of the speed. Y is 0 as jumping isn't enabled in this game.
        }
    }
    
    /*[Command]
    void CmdSetFlipState()
    {
        facingRight = !facingRight; // Change your direction
        RpcChangeFlipStateOnClients();

    }

    [ClientRpc]
    void RpcChangeFlipStateOnClients()
    {
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(s.x * -1, s.y, s.z); // Flip the player when the direction has been switched.
    }*/

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ChatManager.instance.ChatSendMessage(username, "Hey I'm good");
        }

        if (mode == Mode.inGame && !isPaused) // If playing...
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
                    AudioManager.instance.PlaySound(b.blockBreakSoundName);
                    hit.transform.gameObject.SetActive(false); // Temporarily set it to hidden so that even if it takes the server some time to destroy it, you cant mine it twice.
                    CmdBreakBlockOnServer(hit.transform.gameObject);
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

    [Command]
    void CmdBreakBlockOnServer(GameObject obj)
    {
        NetworkServer.Destroy(obj);
        Block b = obj.GetComponent<Block>();
        RpcAddPoints(b.blockPointsValue);
    }

    [ClientRpc]
    void RpcAddPoints(int p)
    {
        points += p;
    }

    [Command]
    public void CmdReachedEnd()
    {
        RpcAddPoints(Mathf.FloorToInt(FindObjectOfType<GameManager>().timeLeft / 4f));
    }

    [Command]
    public void CmdChangeReadyState()
    {
        ready = true;
    }

    void OnDirectionChange(bool facingRight)
    {
        Vector3 s = transform.localScale;
        transform.localScale = new Vector3(s.x * -1, s.y, s.z);
    }
}