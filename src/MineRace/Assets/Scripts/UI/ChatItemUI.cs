using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatItemUI : NetworkBehaviour
{
    private readonly NetworkVariable<NetworkChatMessage> message = new NetworkVariable<NetworkChatMessage>();

    [SerializeField] private Text senderText;
    [SerializeField] private Text messageText;

    public override void OnNetworkSpawn()
    {
        message.OnValueChanged += OnMessageChanged;
    }

    public void Setup(string sender, string message, Color colour)
    {
        this.message.Value = new NetworkChatMessage(sender, message, colour);
    }

    private void OnMessageChanged(NetworkChatMessage previousMessage, NetworkChatMessage newMessage)
    {
        senderText.text = $"[{newMessage.sender}]";
        messageText.text = newMessage.message.ToString();
        messageText.color = newMessage.colour;
    }
}
