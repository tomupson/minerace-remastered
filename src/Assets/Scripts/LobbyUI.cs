using System.Collections.Generic;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    private readonly List<GameObject> gameListItemObjects = new List<GameObject>();

    [SerializeField] private Text statusText;
    [SerializeField] private GameObject gameListItemPrefab;
    [SerializeField] private Transform gameListTransform;
    [SerializeField] private InputField filterMatchField;
    [SerializeField] private Button createMatchButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private CreateMatchUI createMatchPopup;
    [SerializeField] private PasswordedGameUI passwordedGamePopup;

    private void Awake()
    {
        createMatchButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            createMatchPopup.Open();
        });

        refreshButton.onClick.AddListener(() =>
        {
            refreshButton.enabled = false;
            AudioManager.Instance.PlaySound("button_press");
            RefreshGameList();
        });

        passwordedGamePopup.OnClosed += () => RefreshGameList();
    }

    private void Start()
    {
        RefreshGameList();
    }

    private async void RefreshGameList()
    {
        ClearGameList();

        statusText.text = "Searching for open games...";

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
            refreshButton.enabled = true;
            return;
        }

        if (lobbies.Count == 0)
        {
            statusText.text = "No lobbies available.";
            refreshButton.enabled = true;
            return;
        }

        statusText.text = "";

        foreach (Lobby lobby in lobbies)
        {
            GameObject gameListItemObject = Instantiate(gameListItemPrefab);
            gameListItemObject.transform.SetParent(gameListTransform);
            gameListItemObject.GetComponent<GameListItem>().Setup(lobby, JoinLobby);
            gameListItemObjects.Add(gameListItemObject);
        }
    }

    private void ClearGameList()
    {
        foreach (GameObject gameListItemObject in gameListItemObjects)
        {
            Destroy(gameListItemObject);
        }

        gameListItemObjects.Clear();
    }

    private async void JoinLobby(Lobby lobby)
    {
        if (lobby.HasPassword)
        {
            passwordedGamePopup.Show(lobby);
            return;
        }

        bool joined = await LobbyManager.Instance.TryJoinLobby(lobby);
        if (!joined)
        {
            // TODO: Make this a full screen overlay
            statusText.text = "Failed to join lobby.";
            AudioManager.Instance.PlaySound("connection_error");
            ClearGameList();
        }
    }
}
