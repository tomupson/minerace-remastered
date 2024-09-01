using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class PlayerListUI : MonoBehaviour
{
    private readonly Dictionary<ulong, PlayerListItemUI> playerListItems = new();

    [Inject] private readonly NetworkGameState networkGameState;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject playerListItemPrefab;

    private void Start()
    {
        networkGameState.Players.OnListChanged += OnPlayersListChanged;

        foreach (PlayerListState player in networkGameState.Players)
        {
            SpawnPlayerListItem(player);
        }
    }

    private void OnDestroy()
    {
        networkGameState.Players.OnListChanged -= OnPlayersListChanged;
    }

    private void OnPlayersListChanged(NetworkListEvent<PlayerListState> @event)
    {
        switch (@event.Type)
        {
            case NetworkListEvent<PlayerListState>.EventType.Add:
            case NetworkListEvent<PlayerListState>.EventType.Insert:
                SpawnPlayerListItem(@event.Value);
                break;
            case NetworkListEvent<PlayerListState>.EventType.Remove:
            case NetworkListEvent<PlayerListState>.EventType.RemoveAt:
                playerListItems.Remove(@event.Value.clientId, out PlayerListItemUI playerListItem);
                Destroy(playerListItem.gameObject);
                break;
            case NetworkListEvent<PlayerListState>.EventType.Value:
                playerListItems[@event.Value.clientId].Setup(@event.Value.playerName.ToString());
                break;
            case NetworkListEvent<PlayerListState>.EventType.Clear:
                ClearListItems();
                break;
        }
    }

    private void SpawnPlayerListItem(PlayerListState player)
    {
        GameObject playerListItemObject = Instantiate(playerListItemPrefab, content);
        PlayerListItemUI playerListItem = playerListItemObject.GetComponent<PlayerListItemUI>();
        playerListItem.Setup(player.playerName.ToString());

        playerListItems.Add(player.clientId, playerListItem);
    }

    private void ClearListItems()
    {
        foreach (KeyValuePair<ulong, PlayerListItemUI> playerListItem in playerListItems)
        {
            Destroy(playerListItem.Value);
            playerListItems.Remove(playerListItem.Key);
        }
    }
}
