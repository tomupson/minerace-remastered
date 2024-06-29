using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Data.Sqlite;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Handles lots of different functionalities that are mostly things that all players should experience e.g. the game countdown timer.
/// </summary>
public class GameManager : NetworkBehaviour
{
    [Header("Game Settings")]
    public int gameTime = 300;
    public int preGameCountdownTime = 10;

    [Header("Waiting For Players")]
    [SerializeField] private Text timeText;
    [SerializeField] private GameObject waitingForPlayersPanel;
    [SerializeField] private Text waitingForPlayersText;
    [SerializeField] private GameObject pregameTimePanel;
    [SerializeField] private Text pregameTimeText;

    [HideInInspector] public NetworkVariable<int> timeLeft = new NetworkVariable<int>();
    [HideInInspector] public NetworkVariable<int> currentCountdownTimeLeft = new NetworkVariable<int>();
    [HideInInspector] public NetworkVariable<bool> playing = new NetworkVariable<bool>();
    [HideInInspector] public NetworkVariable<bool> preGame = new NetworkVariable<bool>();

    Player[] players;

    private List<string> highScoreQueries = new List<string>();

    void Start()
    {
        playing.Value = false;
        preGame.Value = true;
        pregameTimePanel.SetActive(false);
        timeLeft.Value = gameTime;
        currentCountdownTimeLeft.Value = preGameCountdownTime;
        StartCoroutine(WaitForPlayers());
    }

    void Update()
    {
        if (highScoreQueries.Count >= 2)
        {
            UpdateHighScoresServerRpc();
        }
    }

    public IEnumerator WaitForPlayers()
    {
        waitingForPlayersPanel.SetActive(true);
        int index = 0;
        while (FindObjectsOfType<Player>().Length < 2)
        {
            yield return new WaitForSeconds(1);
            waitingForPlayersText.text = "WAITING FOR PLAYERS" + new string('.', index);
            index++;
            index %= 4;
        }

        waitingForPlayersPanel.SetActive(false);

        players = FindObjectsOfType<Player>();

        foreach (Player p in players)
        {
            p.mode = Player.Mode.ReadyUp;
            p.GetComponentInChildren<PlayerUI>().ShowReadyUp();
        }

        StartCoroutine(WaitForReady());

        yield return null;
    }

    public IEnumerator WaitForReady()
    {
        while (players.Where(z => z.ready.Value).Count() < 2)
        {
            yield return null;
        }

        foreach (Player p in players)
        {
            p.mode = Player.Mode.PregameCountdown;
            p.GetComponentInChildren<PlayerUI>().HideWaitingForPlayerReady();
        }

        StartCoroutine(PreGameCountdown(preGameCountdownTime));

        yield return null;
    }

    public IEnumerator PreGameCountdown(int countdownTime)
    {
        pregameTimePanel.SetActive(true);
        float tL = countdownTime;
        while (tL > 0)
        {
            yield return new WaitForSeconds(1);
            tL--;
            if (IsServer)
                PregameTimeServerRpc();
        }

        foreach (Player p in players)
        {
            p.mode = Player.Mode.InGame;
        }

        StartCoroutine(GameCountdown(gameTime));

        yield return null;
    }

    public IEnumerator GameCountdown(int startTime)
    {
        float tL = startTime;
        while (tL > 0)
        {
            yield return new WaitForSeconds(1);
            tL--;
            if (IsServer)
                TimeServerRpc();
        }

        yield return null;

    }

    [ServerRpc]
    public void GameOverServerRpc()
    {
        GameFinishedClientRpc();
        playing.Value = false;
    }

    [ClientRpc]
    void GameFinishedClientRpc()
    {
        foreach (Player p in players)
        {
            p.mode = Player.Mode.GameOver;
        }

        UIManager ui = FindObjectOfType<UIManager>();
        ui.GameFinished();
    }

    [ServerRpc]
    public void TimeServerRpc() // Send a command to the server to decrease the time.
    {
        timeLeft.Value--;
        DecreaseTimeClientRpc(timeLeft.Value);
    }

    [ServerRpc]
    public void PregameTimeServerRpc()
    {
        currentCountdownTimeLeft.Value--;
        DecreasePregameTimeClientRpc(currentCountdownTimeLeft.Value);
    }

    [ClientRpc]
    void DecreaseTimeClientRpc(int newTime) // Emitt the servers version of the time to all clients.
    {
        if (playing.Value)
        {
            string minutes = Mathf.Floor(newTime / 60).ToString("00");
            string seconds = (newTime % 60).ToString("00");
            timeText.text = minutes + ":" + seconds + " remaining";
            if (newTime <= 0)
            {
                UIManager ui = FindObjectOfType<UIManager>();
                ui.TimesUpServerRpc();
                playing.Value = false;
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    p.mode = Player.Mode.Completed;
                }
            }
        }
    }

    [ClientRpc]
    void DecreasePregameTimeClientRpc(int newTime)
    {
        if (preGame.Value)
        {
            string seconds = newTime.ToString("00");
            pregameTimeText.text = seconds;
            if (newTime <= 0)
            {
                preGame.Value = false;
                playing.Value = true;
                pregameTimeText.enabled = false;
            }
        }
    }

    public void AddHighScoreQuery(int points, long id)
    {
        highScoreQueries.Add(string.Format("UPDATE USERS SET highScore = {0} WHERE id = {1}", points, id));
    }

    [ServerRpc]
    void UpdateHighScoresServerRpc()
    {
        string conn = "URI=file:" + Application.streamingAssetsPath + "/MineRace.db";

        using SqliteConnection dbConnection = new SqliteConnection(conn);
        dbConnection.Open();

        foreach (string query in highScoreQueries)
        {
            IDbCommand command = dbConnection.CreateCommand();

            command.CommandText = query;

            command.ExecuteNonQuery();

            command.Dispose();
        }

        // Cleanup
        highScoreQueries.Clear();
    }
}
