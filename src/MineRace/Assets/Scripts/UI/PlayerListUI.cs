using System.Collections.Generic;
using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class PlayerListUI : MonoBehaviour
{
    private readonly Dictionary<ulong, GameObject> objects = new();

    [Inject] private readonly NetworkManager networkManager;

    private DisposableGroup subscriptions;

    [SerializeField] private Transform content;
    [SerializeField] private GameObject playerListItemPrefab;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<NetworkConnectionEventMessage> connectionEventSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(connectionEventSubscriber.Subscribe(OnConnectionEvent));
    }

    private void Start()
    {

    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnConnectionEvent(NetworkConnectionEventMessage message)
    {
        if (message.connectStatus == ConnectStatus.Success)
        {
            GameObject playerListItemObject = Instantiate(playerListItemPrefab, content);
            PlayerListItemUI playerListItem = playerListItemObject.GetComponent<PlayerListItemUI>();
            playerListItem.Setup(message.playerName.ToString());

            objects.Add(message.clientId, playerListItemObject);
        }
        else if (objects.Remove(message.clientId, out GameObject playerListItemObject))
        {
            Destroy(playerListItemObject);
        }
    }
}
