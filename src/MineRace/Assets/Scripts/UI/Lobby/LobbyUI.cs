using System.Collections.Generic;
using MineRace.Audio;
using MineRace.ConnectionManagement;
using MineRace.UGS;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LobbyUI : MonoBehaviour
{
    private readonly List<GameObject> gameListItemObjects = new List<GameObject>();

    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly LobbyManager lobbyManager;

    [SerializeField] private Text statusText;
    [SerializeField] private GameObject gameListItemPrefab;
    [SerializeField] private Transform gameListTransform;
    [SerializeField] private Button createMatchButton;
    [SerializeField] private Button refreshButton;
    [SerializeField] private CreateMatchUI createMatchPopup;
    [SerializeField] private PasswordedGameUI passwordedGamePopup;

    [SerializeField] private Sound connectionErrorSound;

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

        List<Lobby> lobbies = await lobbyManager.QueryLobbies();
        if (lobbies == null)
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
            GameObject gameListItemObject = Instantiate(gameListItemPrefab, gameListTransform);
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

        bool joined = await lobbyManager.TryJoinLobby(lobby);
        if (!joined)
        {
            AudioManager.PlayOneShot(connectionErrorSound);
            ClearGameList();
            return;
        }

        connectionManager.StartClient();
    }
}
