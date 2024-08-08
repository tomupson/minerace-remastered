using Unity.Netcode;
using UnityEngine;

public class NetworkGameState : NetworkBehaviour
{
    private float ticker = 1f;

    [SerializeField] private int gameTime = 300;
    [SerializeField] private int preGameCountdownTime = 5;

    public NetworkVariable<GameState> State { get; } = new NetworkVariable<GameState>(GameState.WaitingForPlayers);

    public NetworkVariable<int> PregameTimeRemaining { get; } = new NetworkVariable<int>();

    public NetworkVariable<int> TimeRemaining { get; } = new NetworkVariable<int>();

    private void Update()
    {
        if (State.Value == GameState.PregameCountdown)
        {
            ticker -= Time.deltaTime;
            if (ticker <= 0f)
            {
                PregameTimeRemaining.Value--;
                if (PregameTimeRemaining.Value == 0)
                {
                    State.Value = GameState.InGame;
                }

                ticker = 1f;
            }
        }

        if (State.Value == GameState.InGame)
        {
            ticker -= Time.deltaTime;
            if (ticker <= 0f)
            {
                TimeRemaining.Value--;
                if (TimeRemaining.Value == 0)
                {
                    State.Value = GameState.Completed;
                }

                ticker = 1f;
            }
        }
    }

    public override void OnNetworkSpawn()
    {
        if (!IsServer)
        {
            enabled = false;
            return;
        }

        TimeRemaining.Value = gameTime;
        PregameTimeRemaining.Value = preGameCountdownTime;
    }
}
