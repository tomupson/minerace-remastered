using System.Collections;
using System.Collections.Generic;
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

public class JoinGame : MonoBehaviour
{
    private readonly List<GameObject> gameListItems = new List<GameObject>();
    private Lobby selectedLobby;

    [Header("Game List")]
    [SerializeField] private Text statusText;
    [SerializeField] private GameObject gameListItemPrefab;
    [SerializeField] private Transform gameListTransform;
    [SerializeField] private InputField filterMatchField;
    [SerializeField] private Button refreshButton;

    [Header("Passworded Game")]
    [SerializeField] private GameObject passwordCanvas;
    [SerializeField] private InputField matchPasswordInputField;
    [SerializeField] private Text passwordStatusText;
    [SerializeField] private Button joinGamePasswordButton;
    [SerializeField] private Button closeJoinGameButton;

    [Header("Account Info")]
    [SerializeField] private Text loginNameText;
    [SerializeField] private Button logoutButton;

    private void Awake()
    {
        refreshButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            RefreshGameList();
        });

        joinGamePasswordButton.onClick.AddListener(async () =>
        {
            if (matchPasswordInputField.text == null || matchPasswordInputField.text == "")
            {
                AudioManager.Instance.PlaySound("error_message");
                passwordStatusText.text = "The field 'Match Password' is required.";
                return;
            }

            joinGamePasswordButton.enabled = false;

            AudioManager.Instance.PlaySound("button_press");
            ClosePasswordCanvas();

            if (selectedLobby == null)
            {
                return;
            }

            try
            {
                await Lobbies.Instance.JoinLobbyByIdAsync(selectedLobby.Id, new JoinLobbyByIdOptions { Password = matchPasswordInputField.text });
                ClearGameList();
            }
            catch (LobbyServiceException)
            {
                AudioManager.Instance.PlaySound("connection_error");
                statusText.text = "Failed to join lobby.";
                RefreshGameList();
            }
        });

        closeJoinGameButton.onClick.AddListener(() =>
        {
            ClosePasswordCanvas();
        });

        loginNameText.text = $"LOGGED IN AS: {UserAccountManager.Instance.UserInfo.Username}";
        logoutButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Login");
        });

        RefreshGameList();
    }

    private async void RefreshGameList()
    {
        ClearGameList();

        statusText.text = "Searching for open games.";

        QueryLobbiesOptions options = new QueryLobbiesOptions();
        options.Count = 20;
        options.Filters = new List<QueryFilter>
        {
            new QueryFilter(QueryFilter.FieldOptions.Name, filterMatchField.text, QueryFilter.OpOptions.CONTAINS)
        };

        List<Lobby> lobbies;
        try
        {
            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);
            lobbies = response.Results;
        }
        catch (LobbyServiceException)
        {
            statusText.text = "Failed to fetch lobbies.";
            return;
        }

        if (lobbies.Count == 0)
        {
            statusText.text = "No lobbies available.";
            return;
        }

        statusText.text = "";

        foreach (Lobby lobby in lobbies)
        {
            GameObject gameListItemObject = Instantiate(gameListItemPrefab);
            gameListItemObject.transform.SetParent(gameListTransform);

            if (gameListItemObject.TryGetComponent(out GameListItem gameListItem))
            {
                gameListItem.Setup(lobby, JoinLobby);
            }

            gameListItems.Add(gameListItemObject);
        }
    }

    private void ClearGameList()
    {
        foreach (GameObject gameListItemObject in gameListItems)
        {
            Destroy(gameListItemObject);
        }

        gameListItems.Clear();
    }

    private async void JoinLobby(Lobby lobby)
    {
        selectedLobby = lobby;

        if (lobby.HasPassword)
        {
            joinGamePasswordButton.enabled = true;
            passwordCanvas.SetActive(true);
            passwordStatusText.text = "";
            Animation anim = passwordCanvas.transform.GetChild(0).GetComponent<Animation>();
            anim["grow"].speed = 1;
            anim["grow"].time = 0;
            anim.Play("grow");
            return;
        }

        selectedLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
        if (selectedLobby.Data.TryGetValue("JOIN_CODE", out DataObject lobbyData))
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(lobbyData.Value);

            RelayServerData relayData = new RelayServerData(joinAllocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayData);

            ConnectionManager.Instance.StartClient();
        }
    }

    private void ClosePasswordCanvas()
    {
        StartCoroutine(WaitForAnimation(passwordCanvas, passwordCanvas.transform.GetChild(0).GetComponent<Animation>(), "grow"));
    }

    private IEnumerator WaitForAnimation(GameObject canvas, Animation animation, string animationName)
    {
        animation[animationName].speed = -1;
        animation[animationName].time = animation[animationName].length;
        animation.Play(animationName);

        yield return new WaitWhile(() => animation.isPlaying);

        canvas.SetActive(false);

        yield return null;
    }
}
