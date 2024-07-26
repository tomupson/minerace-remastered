using System.Collections.Generic;
using MineRace.ConnectionManagement;
using MineRace.UGS;
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
    [SerializeField] private Button createMatchButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private CreateMatchUI createMatchPopup;
    [SerializeField] private PasswordedGameUI passwordedGamePopup;

    private void Awake()
    {
        createMatchButton.onClick.AddListener(() => createMatchPopup.Open());
        refreshButton.onClick.AddListener(RefreshGameList);
    }

    private void Start()
    {
        passwordedGamePopup.OnClosed += () => RefreshGameList();

        RefreshGameList();
    }

    private async void RefreshGameList()
    {
        ClearGameList();

        refreshButton.enabled = false;
        statusText.text = "Searching for open games...";

        List<Lobby> lobbies;
        try
        {
            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions { Count = 20 };
            queryOptions.Filters = new List<QueryFilter>();
            queryOptions.Filters.Add(new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT));

            QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(queryOptions);
            lobbies = response.Results;
        }
        catch (LobbyServiceException ex)
        {
            Debug.LogException(ex);
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
            gameListItemObject.GetComponent<GameListItemUI>().Setup(lobby, JoinLobby);
            gameListItemObjects.Add(gameListItemObject);
        }

        refreshButton.enabled = true;
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
            passwordedGamePopup.Open(lobby);
            return;
        }

        bool joined = await LobbyManager.Instance.TryJoinLobby(lobby);
        if (!joined)
        {
            AudioManager.Instance.PlaySound("connection_error");
            ClearGameList();
            return;
        }

        ConnectionManager.Instance.StartClient();
    }
}
