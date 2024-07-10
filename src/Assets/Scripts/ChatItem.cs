using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatItem : NetworkBehaviour
{
    private readonly NetworkVariable<FixedString64Bytes> sender = new NetworkVariable<FixedString64Bytes>();
    private readonly NetworkVariable<FixedString512Bytes> message = new NetworkVariable<FixedString512Bytes>();

    [Header("Message Settings")]
    [SerializeField] private Text senderText;
    [SerializeField] private Text messageText;

    public override void OnNetworkSpawn()
    {
        sender.OnValueChanged += OnSenderChanged;
        message.OnValueChanged += OnMessageChanged;
    }

    public override void OnNetworkDespawn()
    {
        sender.OnValueChanged -= OnSenderChanged;
        message.OnValueChanged -= OnMessageChanged;
    }

    public void Setup(string sender, string message)
    {
        this.sender.Value = sender;
        this.message.Value = message;
    }

    private void OnSenderChanged(FixedString64Bytes previousSender, FixedString64Bytes newSender)
    {
        senderText.text = $"[{newSender}]:";
    }

    private void OnMessageChanged(FixedString512Bytes previousMessage, FixedString512Bytes newMessage)
    {
        messageText.text = newMessage.ToString();
    }
}
