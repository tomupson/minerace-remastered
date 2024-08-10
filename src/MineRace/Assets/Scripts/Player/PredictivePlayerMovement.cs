using System.Collections.Generic;
using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using MineRace.Utils.Timers;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PredictivePlayerMovement : NetworkBehaviour
{
    private Rigidbody2D playerRigidbody;
    private float moveHorizontal;

    [SerializeField] private NetworkPlayerState networkPlayerState;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float reconciliationCooldownTime = 1f;
    [SerializeField] private float reconciliationThreshold = 10f;
    [SerializeField] private GameObject serverSquare;
    [SerializeField] private GameObject clientSquare;

    private NetworkTimer timer;
    private const float serverTickRate = 50f;
    private const int bufferSize = 1024;

    private CircularBuffer<PlayerStatePayload> clientStateBuffer;
    private CircularBuffer<PlayerInputPayload> clientInputBuffer;
    private PlayerStatePayload lastServerState;
    private PlayerStatePayload lastProcessedState;

    private CircularBuffer<PlayerStatePayload> serverStateBuffer;
    private Queue<PlayerInputPayload> serverInputQueue;

    private CountdownTimer reconciliationCooldownTimer;

    private void Awake()
    {
        playerRigidbody = GetComponent<Rigidbody2D>();

        timer = new NetworkTimer(serverTickRate);
        clientStateBuffer = new CircularBuffer<PlayerStatePayload>(bufferSize);
        clientInputBuffer = new CircularBuffer<PlayerInputPayload>(bufferSize);

        serverStateBuffer = new CircularBuffer<PlayerStatePayload>(bufferSize);
        serverInputQueue = new Queue<PlayerInputPayload>();

        reconciliationCooldownTimer = new CountdownTimer(reconciliationCooldownTime);
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);
        reconciliationCooldownTimer.Tick(Time.deltaTime);
    }

    private void FixedUpdate()
    {
        while (timer.ShouldTick())
        {
            HandleClientTick();
            HandleServerTick();
        }
    }

    public void SetMoveInput(float moveHorizontal)
    {
        this.moveHorizontal = moveHorizontal;
    }

    private void HandleClientTick()
    {
        if (!IsClient || !IsOwner)
        {
            return;
        }

        int currentTick = timer.CurrentTick;
        int bufferIndex = currentTick % bufferSize;

        if (bufferIndex - 1 >= 0)
        {
            PlayerInputPayload lastInputPayload = clientInputBuffer.Get(bufferIndex - 1);
            if (lastInputPayload.moveHorizontal == moveHorizontal)
            {
                return;
            }
        }

        PlayerInputPayload inputPayload = new PlayerInputPayload(currentTick, moveHorizontal);
        clientInputBuffer.Add(inputPayload, bufferIndex);

        SendPlayerInputServerRpc(inputPayload);

        PlayerStatePayload statePayload = ProcessMovement(inputPayload);
        clientSquare.transform.position = statePayload.position;
        clientStateBuffer.Add(statePayload, bufferIndex);

        HandleServerReconciliation();
    }

    private void HandleServerTick()
    {
        if (!IsServer)
        {
            return;
        }

        int bufferIndex = -1;
        while (serverInputQueue.Count > 0)
        {
            PlayerInputPayload inputPayload = serverInputQueue.Dequeue();

            bufferIndex = inputPayload.tick % bufferSize;

            PlayerStatePayload statePayload = ProcessMovement(inputPayload);
            serverSquare.transform.position = statePayload.position;
            serverStateBuffer.Add(statePayload, bufferIndex);
        }

        if (bufferIndex == -1)
        {
            return;
        }

        SendPlayerStateClientRpc(serverStateBuffer.Get(bufferIndex));
    }

    private PlayerStatePayload ProcessMovement(PlayerInputPayload inputPayload)
    {
        Move(inputPayload.moveHorizontal);
        return new PlayerStatePayload(inputPayload.tick, transform.position, playerRigidbody.velocity);
    }

    private void Move(float moveHorizontal)
    {
        if (networkPlayerState.State.Value != PlayerState.Playing)
        {
            return;
        }

        if (moveHorizontal > 0 && !networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = true;
        }

        if (moveHorizontal < 0 && networkPlayerState.FacingRight.Value)
        {
            networkPlayerState.FacingRight.Value = false;
        }

        playerRigidbody.velocity = new Vector2(moveHorizontal * moveSpeed, 0);
    }

    private void HandleServerReconciliation()
    {
        if (lastServerState.Equals(default(PlayerStatePayload)) || lastProcessedState.Equals(lastServerState) || reconciliationCooldownTimer.IsRunning)
        {
            return;
        }

        int bufferIndex = lastServerState.tick % bufferSize;
        if (bufferIndex - 1 < 0)
        {
            return;
        }

        PlayerStatePayload rewindState = IsHost ? serverStateBuffer.Get(bufferIndex - 1) : lastServerState;
        if (rewindState.tick == 0)
        {
            // Don't try to reconcile against tick 0, as it doesn't represent anything meaningful
            return;
        }

        float positionError = Vector2.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);

        if (positionError > reconciliationThreshold)
        {
            ReconcileState(rewindState);
            reconciliationCooldownTimer.Start();
        }

        lastProcessedState = lastServerState;
    }

    private void ReconcileState(PlayerStatePayload rewindState)
    {
        transform.position = rewindState.position;
        playerRigidbody.velocity = rewindState.velocity;

        if (!rewindState.Equals(lastServerState))
        {
            return;
        }

        clientStateBuffer.Add(rewindState, rewindState.tick);

        int tickToReplay = lastServerState.tick;

        while (tickToReplay < timer.CurrentTick)
        {
            int bufferIndex = tickToReplay % bufferSize;
            PlayerStatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
            clientStateBuffer.Add(statePayload, bufferIndex);
            tickToReplay++;
        }
    }

    [ServerRpc]
    private void SendPlayerInputServerRpc(PlayerInputPayload inputPayload)
    {
        serverInputQueue.Enqueue(inputPayload);
    }

    [ClientRpc]
    private void SendPlayerStateClientRpc(PlayerStatePayload statePayload)
    {
        if (!IsOwner)
        {
            return;
        }

        lastServerState = statePayload;
    }
}
