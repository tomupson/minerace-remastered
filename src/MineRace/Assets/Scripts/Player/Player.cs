using System;
using System.Collections.Generic;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using Unity.Netcode;
using UnityEngine;
using VContainer;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Collider2D))]
public class Player : NetworkBehaviour
{
    private static readonly List<Player> spawnedPlayers = new List<Player>();

    public static event Action<Player> OnLocalPlayerSpawned;

    [Inject] private readonly NetworkGameState networkGameState;

    private Rigidbody2D playerRigidbody;
    private SpriteRenderer playerSpriteRenderer;
    private Collider2D playerCollider;

    private DisposableGroup subscriptions;

    public event Action<Player> OnSpectating;

    [SerializeField] private NetworkPlayerState networkPlayerState;

    public NetworkPlayerState NetworkPlayerState => networkPlayerState;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
    }

    public override void OnDestroy()
    {
        spawnedPlayers.Remove(this);
    }

    public override void OnNetworkSpawn()
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(networkPlayerState.State.Subscribe(OnPlayerStateChanged));
        subscriptions.Add(networkPlayerState.FacingRight.Subscribe(OnFacingRightChanged));

        if (IsLocalPlayer)
        {
            OnLocalPlayerSpawned?.Invoke(this);
        }

        if (!IsServer)
        {
            return;
        }

        subscriptions.Add(networkGameState.State.Subscribe(OnGameStateChanged));

        SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(OwnerClientId);
        if (sessionPlayerData.HasValue)
        {
            networkPlayerState.Username.Value = sessionPlayerData.Value.PlayerName;
        }

        spawnedPlayers.Add(this);
    }

    public override void OnNetworkDespawn()
    {
        subscriptions?.Dispose();

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

    public static IReadOnlyList<Player> GetSpawnedPlayers() => spawnedPlayers.AsReadOnly();

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

    private void OnGameStateChanged(GameState state)
    {
        if (state == GameState.InGame && networkPlayerState.State.Value != PlayerState.Playing)
        {
            NetworkPlayerState.State.Value = PlayerState.Playing;
        }

        if (state == GameState.Completed && networkPlayerState.State.Value != PlayerState.Completed)
        {
            NetworkPlayerState.State.Value = PlayerState.Completed;
        }
    }

    private void OnPlayerStateChanged(PlayerState state)
    {
        if (state == PlayerState.Completed)
        {
            playerSpriteRenderer.enabled = false;
            playerCollider.enabled = false;
            playerRigidbody.bodyType = RigidbodyType2D.Static;
        }
    }

    private void OnFacingRightChanged(bool facingRight)
    {
        playerSpriteRenderer.flipX = !facingRight;
    }
}
