using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using UnityEngine;
using VContainer;

public class ConnectionChatMessageFeed : MonoBehaviour
{
    [Inject] private readonly ChatManager chatManager;

    private DisposableGroup subscriptions;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<NetworkConnectionEventMessage> connectionEventSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(connectionEventSubscriber.Subscribe(OnConnectionEvent));
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnConnectionEvent(NetworkConnectionEventMessage message)
    {
        switch (message.connectStatus)
        {
            case ConnectStatus.Success:
                chatManager.SpawnServerChatMessage($"{message.playerName} has connected", Color.green);
                break;
            case ConnectStatus.GenericDisconnect:
                chatManager.SpawnServerChatMessage($"{message.playerName} has disconnected", Color.red);
                break;
        }
    }
}
