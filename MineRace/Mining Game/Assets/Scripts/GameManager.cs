using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

/// <summary>
/// GameManager.cs handles lots of different functionalities that are mostly things that all players should experience e.g. the game countdown timer.
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
    
    [SyncVar] [HideInInspector] public int timeLeft;
    [SyncVar] [HideInInspector] public int currentCountdownTimeLeft;
    [SyncVar] [HideInInspector] public bool playing = false;
    [SyncVar] [HideInInspector] public bool preGame = false;

    Player[] players;

    private List<string> highScoreQueries = new List<string>();

    void Start()
    {
        playing = false;
        preGame = true;
        pregameTimePanel.SetActive(false);
        timeLeft = gameTime;
        currentCountdownTimeLeft = preGameCountdownTime;
        StartCoroutine(WaitForPlayers());
    }

    void Update()
    {
        if (highScoreQueries.Count >= 2)
        {
            CmdUpdateHighScores();
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
            if (index > 3)
                index = 0;
        }

        waitingForPlayersPanel.SetActive(false);

        players = FindObjectsOfType<Player>();

        foreach (Player p in players)
        {
            p.mode = Player.Mode.readyUp;
            p.GetComponentInChildren<PlayerUI>().ShowReadyUp();
        }

        StartCoroutine(WaitForReady());

        yield return null;
    }

    public IEnumerator WaitForReady()
    {
        while (players.Where(z => z.ready).Count() < 2)
        {
            yield return null;
        }

        foreach (Player p in players)
        {
            p.mode = Player.Mode.pregameCountdown;
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
            if (isServer)
                CmdPregameTime();
        }

        foreach (Player p in players)
        {
            p.mode = Player.Mode.inGame;
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
            if (isServer)
                CmdTime();
        }

        yield return null;

    }

    [Command]
    public void CmdGameOver()
    {
        RpcGameFinished();
        playing = false;
    }

    [ClientRpc]
    void RpcGameFinished()
    {
        foreach (Player p in players)
        {
            p.mode = Player.Mode.gameOver;
        }

        UIManager ui = FindObjectOfType<UIManager>();
        ui.GameFinished();
    }

    [Command]
    public void CmdTime() // Send a command to the server to decrease the time.
    {
        timeLeft--;
        RpcDecreaseTime(timeLeft);
    }

    [Command]
    public void CmdPregameTime()
    {
        currentCountdownTimeLeft--;
        RpcDecreasePregameTime(currentCountdownTimeLeft);
    }

    [ClientRpc]
    void RpcDecreaseTime(int newTime) // Emitt the servers version of the time to all clients.
    {
        if (playing)
        {
            string minutes = Mathf.Floor(newTime / 60).ToString("00");
            string seconds = (newTime % 60).ToString("00");
            timeText.text = minutes + ":" + seconds + " remaining";
            if (newTime <= 0)
            {
                UIManager ui = FindObjectOfType<UIManager>();
                ui.CmdTimesUp();
                playing = false;
                foreach (Player p in FindObjectsOfType<Player>())
                {
                    p.mode = Player.Mode.completed;
                }
            }
        }
    }

    [ClientRpc]
    void RpcDecreasePregameTime(int newTime)
    {
        if (preGame)
        {
            string seconds = newTime.ToString("00");
            pregameTimeText.text = seconds;
            if (newTime <= 0)
            {
                preGame = false;
                playing = true;
                pregameTimeText.enabled = false;
            }
        }
    }

    public void AddHighScoreQuery(int points, long id)
    {
        highScoreQueries.Add(string.Format("UPDATE USERS SET highScore = {0} WHERE id = {1}", points, id));
    }

    [Command]
    void CmdUpdateHighScores()
    {
        string conn = "URI=file:" + Application.streamingAssetsPath + "/MineRace.db";

        IDbConnection dbConnection;
        dbConnection = (IDbConnection)new SqliteConnection(conn);
        dbConnection.Open();

        foreach (string query in highScoreQueries)
        {
            IDbCommand command = dbConnection.CreateCommand();

            command.CommandText = query;

            command.ExecuteNonQuery();

            command.Dispose();
            command = null;
        }

        // Cleanup
        highScoreQueries.Clear();
        dbConnection.Close();
        dbConnection = null;
    }
}
