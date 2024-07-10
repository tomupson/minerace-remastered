using System;
using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private bool listenForSpaceKey = false;

    [Header("Screenspace")]
    [SerializeField] private GameObject screenspacePanel;
    [SerializeField] private Text pointsText;
    [SerializeField] private Text timeText;

    [Header("Waiting For Players")]
    [SerializeField] private GameObject waitingForPlayersPanel;
    [SerializeField] private Text waitingForPlayersText;

    [Header("Waiting For Ready")]
    [SerializeField] private GameObject waitingForPlayerReadyPanel;
    [SerializeField] private Text waitingForPlayerReadyText;

    [Header("Ready")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private Button readyButton;

    [Header("Pregame")]
    [SerializeField] private GameObject preGameTimePanel;
    [SerializeField] private Text preGameTimeText;

    [Header("Spectating")]
    [SerializeField] private Text youAreSpectatingText;
    [SerializeField] private Text spectateText;

    [Header("Times Up")]
    [SerializeField] private GameObject timesUpPanel;
    [SerializeField] private Text timesUpText;

    [Header("Final Scores")]
    [SerializeField] private GameObject finalScoresPanel;
    [SerializeField] private Text[] playerNameText;
    [SerializeField] private Text[] finalPointsText;
    [SerializeField] private Text[] finalMoneyText;

    [Header("Game Finished")]
    [SerializeField] private GameObject gameFinishedPanel;
    [SerializeField] private Text gameFinishedText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button leaveButton;

    private void Start()
    {
        GameManager.Instance.State.OnValueChanged += HandleGameStateChanged;
        GameManager.Instance.TimeRemaining.OnValueChanged += HandleTimeRemainingChanged;
        GameManager.Instance.PreGameTimeRemaining.OnValueChanged += HandlePreGameTimeRemainingChanged;
        Player.OnAnyPlayerSpawned += OnAnyPlayedSpawned;

        waitingForPlayersPanel.SetActive(true);
        StartCoroutine(WaitForPlayers());

        readyButton.onClick.AddListener(() =>
        {
            readyPanel.SetActive(false);
            readyButton.enabled = false;

            waitingForPlayerReadyPanel.SetActive(true);

            Player.LocalPlayer.SetModeServerRpc(PlayerState.WaitingForPlayerReady);
            StartCoroutine(UpdateWaitingForReadyText());
        });

        leaveButton.onClick.AddListener(() =>
        {
            NetworkManager.Singleton.Shutdown();
        });
    }

    private void OnAnyPlayedSpawned(object sender, EventArgs e)
    {
        if (Player.LocalPlayer != null)
        {
            Player.LocalPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;
            Player.LocalPlayer.Points.OnValueChanged += HandlePlayerPointsChanged;

            Player.LocalPlayer.State.OnValueChanged -= HandlePlayerStateChanged;
            Player.LocalPlayer.State.OnValueChanged += HandlePlayerStateChanged;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Player.LocalPlayer.isPaused)
            {
                UnpauseGame();
            }
            else
            {
                PauseGame();
            }
        }

        if (listenForSpaceKey && Input.GetKeyDown(KeyCode.Space))
        {
            Player[] players = FindObjectsOfType<Player>();
            Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);
            otherPlayer.GetComponentInChildren<Camera>().enabled = true;
            otherPlayer.GetComponentInChildren<AudioListener>().enabled = true;

            Player.LocalPlayer.GetComponentInChildren<Camera>().enabled = false;
            Player.LocalPlayer.GetComponentInChildren<AudioListener>().enabled = false;

            Player.LocalPlayer.SetModeServerRpc(PlayerState.Spectating);
            listenForSpaceKey = false;
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.State.OnValueChanged -= HandleGameStateChanged;
        GameManager.Instance.TimeRemaining.OnValueChanged -= HandleTimeRemainingChanged;
        GameManager.Instance.PreGameTimeRemaining.OnValueChanged -= HandlePreGameTimeRemainingChanged;
        Player.LocalPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;
        Player.LocalPlayer.State.OnValueChanged -= HandlePlayerStateChanged;
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.Completed && previousState != GameState.Completed)
        {
            if (GameManager.Instance.TimeRemaining.Value == 0)
            {
                timesUpPanel.SetActive(true);
                timesUpText.GetComponent<Animation>().Play("timesUp");
            }
            else
            {
                gameFinishedPanel.SetActive(true);
                gameFinishedText.GetComponent<Animation>().Play("timesUp");
            }

            StartCoroutine(OpenScoresPanel());
        }
    }

    private void HandleTimeRemainingChanged(int previousTime, int newTime)
    {
        string minutes = Mathf.Floor(newTime / 60).ToString("00");
        string seconds = (newTime % 60).ToString("00");
        timeText.text = $"{minutes}:{seconds} remaining";
    }

    private void HandlePreGameTimeRemainingChanged(int previousTime, int newTime)
    {
        string seconds = newTime.ToString("00");
        preGameTimeText.text = seconds;
    }

    private void HandlePlayerPointsChanged(int previousPoints, int newPoints)
    {
        pointsText.text = $"POINTS: {newPoints}";
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.ReadyUp:
                waitingForPlayersPanel.SetActive(false);
                readyPanel.SetActive(true);
                break;
            case PlayerState.PregameCountdown:
                waitingForPlayerReadyPanel.SetActive(false);
                preGameTimePanel.SetActive(true);
                break;
            case PlayerState.InGame:
                preGameTimePanel.SetActive(false);
                break;
            case PlayerState.Spectating:
                spectateText.enabled = false;
                youAreSpectatingText.enabled = true;

                Player otherPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => !p.IsLocalPlayer);
                pointsText.text = $"POINTS: {otherPlayer.Points.Value}";
                youAreSpectatingText.text = $"YOU ARE SPECTATING: {otherPlayer.Username.Value}";
                break;
            case PlayerState.Completed when GameManager.Instance.State.Value == GameState.Playing:
                spectateText.enabled = true;
                listenForSpaceKey = true;
                break;
            case PlayerState.Completed:
                timeText.enabled = false;
                listenForSpaceKey = false;
                // TODO: Update high score
                break;
        }
    }

    private void PauseGame()
    {
        switch (Player.LocalPlayer.State.Value)
        {
            case PlayerState.ReadyUp:
                readyPanel.SetActive(false);
                break;
            case PlayerState.WaitingForPlayerReady:
                waitingForPlayerReadyPanel.SetActive(false);
                break;
        }

        pauseMenu.SetActive(true);
        Player.LocalPlayer.isPaused = true;
    }

    private void UnpauseGame()
    {
        Player.LocalPlayer.isPaused = false;
        pauseMenu.SetActive(false);

        switch (Player.LocalPlayer.State.Value)
        {
            case PlayerState.ReadyUp:
                readyPanel.SetActive(true);
                break;
            case PlayerState.WaitingForPlayerReady:
                waitingForPlayerReadyPanel.SetActive(true);
                break;
        }
    }

    private IEnumerator WaitForPlayers()
    {
        int index = 0;
        while (NetworkManager.Singleton.ConnectedClientsIds.Count < 2)
        {
            yield return new WaitForSeconds(1);
            waitingForPlayersText.text = $"WAITING FOR PLAYERS{new string('.', index)}";
            index++;
            index %= 4;
        }
    }

    private IEnumerator UpdateWaitingForReadyText()
    {
        while (Player.LocalPlayer.State.Value == PlayerState.WaitingForPlayerReady)
        {
            Player otherPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => !p.IsLocalPlayer);
            waitingForPlayerReadyText.text = $"WAITING FOR '{otherPlayer.Username.Value}' TO READY UP.";
            yield return null;
        }
    }

    private IEnumerator OpenScoresPanel()
    {
        yield return new WaitForSeconds(3);

        Player[] players = FindObjectsOfType<Player>();
        for (int i = 0; i < players.Length; i++)
        {
            playerNameText[i].enabled = false;
            finalPointsText[i].enabled = false;
            finalMoneyText[i].enabled = false;
        }

        timesUpText.enabled = false;
        gameFinishedPanel.SetActive(false);
        finalScoresPanel.SetActive(true);

        finalScoresPanel.GetComponent<Animation>().Play("openFinalScoresPanel");
        yield return new WaitUntil(() => !finalScoresPanel.GetComponent<Animation>().isPlaying);

        for (int i = 0; i < players.Length; i++)
        {
            playerNameText[i].enabled = true;
            playerNameText[i].text = $"Player: {players[i].Username.Value}";
            playerNameText[i].GetComponent<Animation>().Play("dropIn");
            yield return new WaitUntil(() => !playerNameText[i].GetComponent<Animation>().isPlaying);
            finalPointsText[i].enabled = true;
            int counted = 0;

            while (counted < players[i].Points.Value)
            {
                counted += (int)(players[i].Points.Value / 1 * Time.deltaTime);
                finalPointsText[i].text = $"Points: +{counted}";
                yield return null;
            }

            finalMoneyText[i].enabled = true;

            finalMoneyText[i].text = $"Points: +{counted}";
            counted = 0;
            while (counted < Mathf.FloorToInt(players[i].Points.Value / 4))
            {
                if (counted % 4 == 0)
                {
                    AudioManager.Instance.PlaySound("money_gain");
                }

                counted += (int)((players[i].Points.Value / 4) / 1 * Time.deltaTime);
                finalMoneyText[i].text = $"Money: +${counted}";
                yield return null;
            }

            finalMoneyText[i].text = $"Money: +${counted}";
        }
    }
}
