using System;
using MineRace.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(CircleCollider2D))]
public class Player : NetworkBehaviour
{
    public static event Action<Player> OnLocalPlayerSpawned;

    [Inject] private readonly NetworkGameState networkGameState;

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer spriteRenderer;
    private CircleCollider2D circleCollider;

    public event Action<Player> OnSpectating;

    [SerializeField] private NetworkPlayerState networkPlayerState;

    public NetworkPlayerState NetworkPlayerState => networkPlayerState;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            networkGameState.State.OnValueChanged += OnGameStateChanged;

            SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(OwnerClientId);
            if (sessionPlayerData.HasValue)
            {
                networkPlayerState.Username.Value = sessionPlayerData.Value.PlayerName;
            }
        }

        if (IsLocalPlayer)
        {
            OnLocalPlayerSpawned?.Invoke(this);
        }

        networkPlayerState.State.OnValueChanged += OnPlayerStateChanged;
        networkPlayerState.FacingRight.OnValueChanged += OnFacingRightChanged;
    }

    public override void OnNetworkDespawn()
    {
        if (!IsServer)
        {
            return;
        }

        SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(OwnerClientId);
        if (!sessionPlayerData.HasValue)
        {
            return;
        }

        SessionPlayerData playerData = sessionPlayerData.Value;
        playerData.PlayerPosition = transform.position;
        playerData.HasCharacterSpawned = true;
        SessionManager.Instance.SetPlayerData(OwnerClientId, playerData);
    }

    public void BreakBlock(GameObject block)
    {
        BreakBlockServerRpc(block);
    }

    public void ReachedEnd()
    {
        ReachedEndServerRpc();
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
        if (networkPlayerState.State.Value >= state)
        {
            return;
        }

        networkPlayerState.State.Value = state;
    }

    [ServerRpc]
    private void BreakBlockServerRpc(NetworkObjectReference reference)
    {
        // TODO: Validate the block break
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.Despawn();

            Block block = networkObject.GetComponent<BlockRenderer>().block;
            networkPlayerState.Points.Value += block.pointsValue;
        }
    }

    [ServerRpc]
    private void ReachedEndServerRpc()
    {
        networkPlayerState.State.Value = PlayerState.Completed;

        int timeRemainingSeconds = networkGameState.TimeRemaining.Value;
        networkPlayerState.Points.Value += Mathf.FloorToInt(timeRemainingSeconds / 4f);
    }

    private void OnGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.InGame && networkPlayerState.State.Value != PlayerState.Playing)
        {
            NetworkPlayerState.State.Value = PlayerState.Playing;
        }

        if (newState == GameState.Completed && networkPlayerState.State.Value != PlayerState.Completed)
        {
            NetworkPlayerState.State.Value = PlayerState.Completed;
        }
    }

    private void OnPlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        if (newState == PlayerState.Completed)
        {
            spriteRenderer.enabled = false;
            circleCollider.enabled = false;
            playerRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnFacingRightChanged(bool previousFacingRight, bool newFacingRight)
    {
        spriteRenderer.flipX = !newFacingRight;
    }
}
