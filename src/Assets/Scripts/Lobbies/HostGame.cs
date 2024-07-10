using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HostGame : MonoBehaviour
{
    [Header("Match Settings")]
    [SerializeField] private int gameSize = 2;

    [Header("Create Match References")]
    [SerializeField] private GameObject createMatchCanvas;
    [SerializeField] private InputField gameNameField;
    [SerializeField] private InputField gamePasswordField;
    [SerializeField] private Text createGameStatusText;
    [SerializeField] private Button openCreateMatchButton;
    [SerializeField] private Button closeCreateMatchButton;
    [SerializeField] private Button createGameButton;

    private void Start()
    {
        openCreateMatchButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            createMatchCanvas.SetActive(true);
            Animation anim = createMatchCanvas.transform.GetChild(0).GetComponent<Animation>();
            anim["grow"].speed = 1;
            anim["grow"].time = 0;
            anim.Play("grow");
            createGameStatusText.text = "";
        });

        closeCreateMatchButton.onClick.AddListener(() =>
        {
            StartCoroutine(WaitForAnimation());
        });

        createGameButton.onClick.AddListener(async () =>
        {
            string gameName = gameNameField.text;
            string gamePassword = gamePasswordField.text;

            if (!string.IsNullOrWhiteSpace(gameName))
            {
                createGameButton.enabled = false;
                AudioManager.Instance.PlaySound("button_press");

                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(gameSize);

                RelayServerData relayData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

                ConnectionManager.Instance.StartHost();

                CreateLobbyOptions options = new CreateLobbyOptions();
                if (!string.IsNullOrWhiteSpace(gamePassword))
                {
                    options.Password = gamePassword;
                }

                string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                options.Data = new Dictionary<string, DataObject>();
                options.Data.Add("JOIN_CODE", new DataObject(DataObject.VisibilityOptions.Member, joinCode));

                Lobby lobby = await Lobbies.Instance.CreateLobbyAsync(gameName, gameSize, options);
                StartCoroutine(LobbyKeepAlive(lobby.Id));
                NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
            }
            else
            {
                AudioManager.Instance.PlaySound("error_message");
                createGameStatusText.text = "The field 'Game Name' is required.";
            }
        });
    }

    private IEnumerator LobbyKeepAlive(string lobbyId)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(2);
        while (true)
        {
            Task heartbeatTask = Task.Run(async () => await Lobbies.Instance.SendHeartbeatPingAsync(lobbyId));
            yield return new WaitUntil(() => heartbeatTask.IsCompleted);
            yield return delay;
        }
    }

    private IEnumerator WaitForAnimation()
    {
        Animation anim = createMatchCanvas.transform.GetChild(0).GetComponent<Animation>();
        FindObjectOfType<ButtonHighlight>().GetComponentInChildren<Text>().color = Color.white;
        anim["grow"].speed = -1;
        anim["grow"].time = anim["grow"].length;
        anim.Play("grow");

        yield return new WaitWhile(() => anim.isPlaying);

        createMatchCanvas.SetActive(false);

        yield return null;
    }
}
