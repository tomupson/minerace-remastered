// TODO: NETWORKING
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatItem : NetworkBehaviour
{
    private readonly NetworkVariable<string> sender = new NetworkVariable<string>();
    private readonly NetworkVariable<string> message = new NetworkVariable<string>();

    [Header("Message Settings")]
    [SerializeField] private Text senderText;
    [SerializeField] private Text messageText;

    //[SyncVar] public NetworkInstanceId chatNetId; // Network ID of the chat box.

    //void Start()
    //{
    //    GameObject chatParent = ClientScene.FindLocalObject(chatNetId);
    //    transform.SetParent(chatParent.transform);
    //    senderText.text = $"[{sender}]:";
    //    messageText.text = message.Value;
    //}

    public void Setup(string sender, string message)
    {
        senderText.text = $"[{sender}]:";
        messageText.text = message;
        this.sender.Value = sender;
        this.message.Value = message;
    }
}
