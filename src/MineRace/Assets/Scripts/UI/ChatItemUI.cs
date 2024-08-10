using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : NetworkBehaviour
{
    private readonly NetworkVariable<NetworkChatMessage> message = new NetworkVariable<NetworkChatMessage>();

    private DisposableGroup subscriptions;

    [SerializeField] private Text senderText;
    [SerializeField] private Text messageText;

    public override void OnNetworkSpawn()
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(message.Subscribe(OnMessageChanged));
    }

    public override void OnNetworkDespawn()
    {
        subscriptions?.Dispose();
    }

    public void Setup(string sender, string message, Color colour)
    {
        this.message.Value = new NetworkChatMessage(sender, message, colour);
    }

    private void OnMessageChanged(NetworkChatMessage message)
    {
        senderText.text = $"[{message.sender}]";
        messageText.text = message.message.ToString();
        messageText.color = message.colour;
    }
}
