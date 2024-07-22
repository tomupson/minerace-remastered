using System.Collections;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private bool listenForSpaceKey = false;

    [Header("Screenspace")]
    [SerializeField] private GameObject screenspacePanel;
    [SerializeField] private Text pointsText;
    [SerializeField] private Text timeText;

    [Header("Waiting For Players")]
    [SerializeField] private Text waitingForPlayersText;

    [Header("Waiting For Ready")]
    [SerializeField] private Text waitingForPlayerReadyText;

    [Header("Ready")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private Button readyButton;

    [Header("Pregame")]
    [SerializeField] private Text preGameTimeText;

    [Header("Spectating")]
    [SerializeField] private Text youAreSpectatingText;
    [SerializeField] private Text spectateText;

    [Header("Times Up")]
    [SerializeField] private Text timesUpText;

    [Header("Final Scores")]
    [SerializeField] private GameObject finalScoresPanel;
    [SerializeField] private Text[] playerNameText;
    [SerializeField] private Text[] finalPointsText;
    [SerializeField] private Text[] finalMoneyText;

    [Header("Game Finished")]
    [SerializeField] private Text gameFinishedText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        GameManager.Instance.State.OnValueChanged += HandleGameStateChanged;
        GameManager.Instance.TimeRemaining.OnValueChanged += HandleTimeRemainingChanged;
        GameManager.Instance.PregameTimeRemaining.OnValueChanged += HandlePregameTimeRemainingChanged;
        Player.OnAnyPlayerSpawned += OnAnyPlayedSpawned;

        readyButton.onClick.AddListener(() =>
        {
            readyPanel.SetActive(false);
            waitingForPlayerReadyText.gameObject.SetActive(true);

            Player otherPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => !p.IsLocalPlayer);
            waitingForPlayerReadyText.text = $"WAITING FOR '{otherPlayer.Username.Value}' TO READY UP.";

            Player.LocalPlayer.Ready();
        });

        leaveButton.onClick.AddListener(() => NetworkManager.Singleton.Shutdown());

        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.PauseUnpause.performed += OnPauseUnpausePerformed;
        inputActions.Player.Spectate.performed += OnSpectatePerformed;
    }

    private void Start()
    {
        waitingForPlayersText.gameObject.SetActive(true);
        StartCoroutine(WaitForPlayers());
    }

    private void OnDestroy()
    {
        Player.OnAnyPlayerSpawned -= OnAnyPlayedSpawned;

        inputActions.Player.PauseUnpause.performed -= OnPauseUnpausePerformed;
        inputActions.Player.Spectate.performed -= OnSpectatePerformed;
        inputActions.Dispose();
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.WaitingForPlayersReady)
        {
            waitingForPlayersText.gameObject.SetActive(false);
            readyPanel.SetActive(true);
        }

        if (newState == GameState.PregameCountdown)
        {
            waitingForPlayerReadyText.gameObject.SetActive(false);
            preGameTimeText.gameObject.SetActive(true);
        }

        if (newState == GameState.InGame)
        {
            screenspacePanel.SetActive(true);
            preGameTimeText.gameObject.SetActive(false);
        }

        if (newState == GameState.Completed && previousState != GameState.Completed)
        {
            if (GameManager.Instance.TimeRemaining.Value == 0)
            {
                timesUpText.gameObject.SetActive(true);
                timesUpText.GetComponent<Animation>().Play("timesUp");
            }
            else
            {
                gameFinishedText.gameObject.SetActive(true);
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

    private void HandlePregameTimeRemainingChanged(int previousTime, int newTime)
    {
        string seconds = newTime.ToString("00");
        preGameTimeText.text = seconds;
    }

    private void OnAnyPlayedSpawned(Player player)
    {
        if (Player.LocalPlayer != null)
        {
            Player.LocalPlayer.Points.OnValueChanged -= HandlePlayerPointsChanged;
            Player.LocalPlayer.Points.OnValueChanged += HandlePlayerPointsChanged;

            Player.LocalPlayer.State.OnValueChanged -= HandlePlayerStateChanged;
            Player.LocalPlayer.State.OnValueChanged += HandlePlayerStateChanged;
        }
    }

    private void HandlePlayerPointsChanged(int previousPoints, int newPoints)
    {
        pointsText.text = $"POINTS: {newPoints}";
    }

    private void HandlePlayerStateChanged(PlayerState previousState, PlayerState newState)
    {
        switch (newState)
        {
            case PlayerState.Completed when GameManager.Instance.State.Value == GameState.InGame:
                spectateText.gameObject.SetActive(true);
                listenForSpaceKey = true;
                break;
            case PlayerState.Completed:
                screenspacePanel.SetActive(false);
                spectateText.gameObject.SetActive(false);
                youAreSpectatingText.gameObject.SetActive(false);
                listenForSpaceKey = false;
                break;
        }
    }

    private void OnPauseUnpausePerformed(InputAction.CallbackContext context)
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

    private void PauseGame()
    {
        if (GameManager.Instance.State.Value == GameState.WaitingForPlayersReady)
        {
            if (Player.LocalPlayer.State.Value == PlayerState.Ready)
            {
                waitingForPlayerReadyText.gameObject.SetActive(false);
            }
            else
            {
                readyPanel.SetActive(false);
            }
        }

        pauseMenu.SetActive(true);
        Player.LocalPlayer.isPaused = true;
    }

    private void UnpauseGame()
    {
        Player.LocalPlayer.isPaused = false;
        pauseMenu.SetActive(false);

        if (GameManager.Instance.State.Value == GameState.WaitingForPlayersReady)
        {
            if (Player.LocalPlayer.State.Value == PlayerState.Ready)
            {
                waitingForPlayerReadyText.gameObject.SetActive(true);
            }
            else
            {
                readyPanel.SetActive(true);
            }
        }
    }

    private void OnSpectatePerformed(InputAction.CallbackContext context)
    {
        if (!listenForSpaceKey)
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();
        Player otherPlayer = players.FirstOrDefault(p => !p.IsLocalPlayer);

        Player.LocalPlayer.Spectate(otherPlayer);

        spectateText.gameObject.SetActive(false);
        youAreSpectatingText.gameObject.SetActive(true);
        youAreSpectatingText.text = $"YOU ARE SPECTATING: {otherPlayer.Username.Value}";

        otherPlayer.Points.OnValueChanged += HandlePlayerPointsChanged;
        HandlePlayerPointsChanged(0, otherPlayer.Points.Value);

        listenForSpaceKey = false;
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

        timesUpText.gameObject.SetActive(false);
        gameFinishedText.gameObject.SetActive(false);
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

                counted += (int)(players[i].Points.Value / 4 / 1 * Time.deltaTime);
                finalMoneyText[i].text = $"Money: +${counted}";
                yield return null;
            }

            finalMoneyText[i].text = $"Money: +${counted}";
        }
    }
}
