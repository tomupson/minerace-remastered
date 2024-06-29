using System.Collections;
using System.Data;
using System.Linq;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : NetworkBehaviour
{
    private Player player;
    private bool updatedHighScore = false;

    [Header("Screen Space UI")]
    [SerializeField] private Text pointsText;

    [Header("Username Overhead UI")]
    [SerializeField] private GameObject usernameCanvas;
    [SerializeField] private Text usernameText;
    [SerializeField] private Text spectateText;

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

        usernameText.text = player.username.Value;

        switch (player.mode)
        {
            case Player.Mode.InGame:
                if (!usernameCanvas.activeSelf)
                {
                    usernameCanvas.SetActive(true);
                }

                usernameCanvas.GetComponent<RectTransform>().position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, 0);

                pointsText.text = "POINTS: " + player.points;

                spectateText.enabled = false;
                break;
            case Player.Mode.Completed when FindObjectOfType<GameManager>().playing.Value:
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

                        player.mode = Player.Mode.Spectating;
                    }

                    break;
                }
            case Player.Mode.Spectating:
                {
                    spectateText.enabled = false;
                    youAreSpectatingText.enabled = true;
                    Player otherPlayer = FindObjectsOfType<Player>().Where(z => z != player).FirstOrDefault();
                    pointsText.text = "POINTS: " + otherPlayer.points;
                    youAreSpectatingText.text = "YOU ARE SPECTATING: " + otherPlayer.username;
                    break;
                }
            case Player.Mode.WaitingForPlayers:
                usernameCanvas.SetActive(false);
                break;
            case Player.Mode.Completed:
                if (player.points.Value > UserAccountManager.Instance.userInfo.HighScore)
                {
                    if (!updatedHighScore)
                    {
                        if (player.IsLocalPlayer)
                        {
                            FindObjectOfType<GameManager>().AddHighScoreQuery(player.points.Value, UserAccountManager.Instance.userInfo.UserId);
                            updatedHighScore = true;
                        }
                    }
                }
                break;
            default:
                spectateText.enabled = false;
                youAreSpectatingText.enabled = false;
                break;
        }
    }

    public void ReadyButtonPressed()
    {
        readyPanel.SetActive(false);
        readyButton.enabled = false;

        player.mode = Player.Mode.WaitingForPlayerReady;

        player.ChangeReadyStateServerRpc();
        waitingForPlayerReadyPanel.SetActive(true);
        StartCoroutine(UpdateWaitingForReadyText());
    }

    IEnumerator UpdateWaitingForReadyText()
    {
        while (player.mode == Player.Mode.ReadyUp)
        {
            Player otherPlayer = FindObjectsOfType<Player>().Where(z => z != player).FirstOrDefault();
            waitingForPlayerReadyText.text = $"WAITING FOR '{otherPlayer.username}' TO READY UP.";
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
            case Player.Mode.ReadyUp:
                pauseMenu.SetActive(true);
                readyPanel.SetActive(false);
                break;
            case Player.Mode.WaitingForPlayerReady:
                pauseMenu.SetActive(true);
                waitingForPlayerReadyPanel.SetActive(false);
                break;
            default:
            case Player.Mode.InGame:
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
            case Player.Mode.ReadyUp:
                pauseMenu.SetActive(false);
                readyPanel.SetActive(true);
                break;
            case Player.Mode.WaitingForPlayerReady:
                pauseMenu.SetActive(false);
                waitingForPlayerReadyPanel.SetActive(true);
                break;
            default:
            case Player.Mode.InGame:
                pauseMenu.SetActive(false);
                break;
        }
    }

    public void LeaveButtonPressed()
    {
        NetworkManager.Singleton.Shutdown();
    }
}