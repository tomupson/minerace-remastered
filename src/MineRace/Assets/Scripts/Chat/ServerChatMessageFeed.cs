using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ServerChatMessageFeed : NetworkBehaviour
{
    [Inject] private readonly ChatManager chatManager;

    private DisposableGroup subscriptions;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<ConnectionEventMessage> connectionEventSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(connectionEventSubscriber.Subscribe(OnConnectionEvent));
    }

    public override void OnNetworkSpawn()
    {
        enabled = IsServer;
    }

    public override void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnConnectionEvent(ConnectionEventMessage message)
    {
        switch (message.ConnectStatus)
        {
            case ConnectStatus.Success:
                chatManager.SendServerChatMessage($"{message.PlayerName} has connected", Color.green);
                break;
            case ConnectStatus.GenericDisconnect:
                chatManager.SendServerChatMessage($"{message.PlayerName} has disconnected", Color.red);
                break;
        }
    }
}
