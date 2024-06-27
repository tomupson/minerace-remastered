using System.Linq;
using System.Data;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Mono.Data.Sqlite;

public class PlayerUI : NetworkBehaviour
{
    [Header("Screen Space UI")]
    [SerializeField] private Text pointsText;

    [Header("Username Overhead UI")]
    [SerializeField] private GameObject usernameCanvas;
    [SerializeField] private Text usernameText;
    public Text spectateText;

    [Header("Ready Button")]
    [SerializeField] private GameObject readyPanel;
    [SerializeField] private Button readyButton;

    [Header("Waiting For Ready")]
    [SerializeField] private GameObject waitingForPlayerReadyPanel;
    [SerializeField] private Text waitingForPlayerReadyText;

    [Header("Pause Menu")]
    [SerializeField] private GameObject pauseMenu;

    [Header("Spectating")]
    [SerializeField] private Text youAreSpectatingText;

    private Player player;
    private Vector3 oldPos;

    private bool updatedHighScore = false;

    void Start()
    {
        player = GetComponentInParent<Player>();

        spectateText.enabled = false;
        youAreSpectatingText.enabled = false;

        usernameCanvas.SetActive(false);
        readyPanel.SetActive(false);
        waitingForPlayerReadyPanel.SetActive(false);
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (player.isPaused) UnPauseGame();
            else PauseGame();
        }

        usernameText.text = player.username;

        if (player.mode == Player.Mode.inGame)
        {
            if (!usernameCanvas.activeSelf) usernameCanvas.SetActive(true);

            usernameCanvas.GetComponent<RectTransform>().position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, 0);

            pointsText.text = "POINTS: " + player.points;

            spectateText.enabled = false;
        } else if (player.mode == Player.Mode.completed && FindObjectOfType<GameManager>().playing)
        {
            spectateText.enabled = true;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Player[] players = FindObjectsOfType<Player>();
                Player otherPlayer = players.Where(z => z != player).FirstOrDefault();
                otherPlayer.GetComponentInChildren<Camera>().enabled = true;
                otherPlayer.GetComponentInChildren<AudioListener>().enabled = true;

                player.GetComponentInChildren<Camera>().enabled = false;
                player.GetComponentInChildren<AudioListener>().enabled = false;

                player.mode = Player.Mode.spectating;
            }
        } else if (player.mode == Player.Mode.spectating)
        {
            spectateText.enabled = false;
            youAreSpectatingText.enabled = true;
            Player otherPlayer = FindObjectsOfType<Player>().Where(z => z != player).FirstOrDefault();
            pointsText.text = "POINTS: " + otherPlayer.points;
            youAreSpectatingText.text = "YOU ARE SPECTATING: " + otherPlayer.username;
        } else if (player.mode == Player.Mode.waitingForPlayers)
        {
            usernameCanvas.SetActive(false);
        } else if (player.mode == Player.Mode.completed)
        {
            if (player.points > UserAccountManager.instance.userInfo.HighScore)
            {
                if (!updatedHighScore)
                {
                    if (player.isLocalPlayer)
                    {
                        FindObjectOfType<GameManager>().AddHighScoreQuery(player.points, UserAccountManager.instance.userInfo.UserId);
                        updatedHighScore = true;
                    }
                }
            }
        }
        else
        {
            spectateText.enabled = false;
            youAreSpectatingText.enabled = false;
        }
    }

    public void ReadyButtonPressed()
    {
        readyPanel.SetActive(false);
        readyButton.enabled = false;

        player.mode = Player.Mode.waitingForPlayerReady;

        player.CmdChangeReadyState();
        waitingForPlayerReadyPanel.SetActive(true);
        StartCoroutine(UpdateWaitingForReadyText());
    }

    IEnumerator UpdateWaitingForReadyText()
    {
        while (player.mode == Player.Mode.readyUp)
        {
            waitingForPlayerReadyText.text = "WAITING FOR '" + FindObjectsOfType<Player>().Where(z => z != player).FirstOrDefault().username + "' TO READY UP.";
            yield return null;
        }
    }

    public void HideWaitingForPlayerReady()
    {
        waitingForPlayerReadyPanel.SetActive(false);
    }

    public void ShowReadyUp()
    {
        readyPanel.SetActive(true);
    }
    
    void PauseGame()
    {
        switch (player.mode)
        {
            case Player.Mode.readyUp:
                pauseMenu.SetActive(true);
                readyPanel.SetActive(false);
                break;
            case Player.Mode.waitingForPlayerReady:
                pauseMenu.SetActive(true);
                waitingForPlayerReadyPanel.SetActive(false);
                break;
            default:
            case Player.Mode.inGame:
                pauseMenu.SetActive(true);
                break;
        }

        player.isPaused = true;
    }

    public void UnPauseGame()
    {
        player.isPaused = false;

        switch (player.mode)
        {
            case Player.Mode.readyUp:
                pauseMenu.SetActive(false);
                readyPanel.SetActive(true);
                break;
            case Player.Mode.waitingForPlayerReady:
                pauseMenu.SetActive(false);
                waitingForPlayerReadyPanel.SetActive(true);
                break;
            default:
            case Player.Mode.inGame:
                pauseMenu.SetActive(false);
                break;
        }
    }

    public void LeaveButtonPressed()
    {
        NetworkManager netMan = NetworkManager.singleton;
        if (player.isServer)
            netMan.StopHost();
        else
            netMan.StopClient();
    }
}