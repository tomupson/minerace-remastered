// TODO: NETWORKING
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ChatItem : NetworkBehaviour
{
    [Header("Message Settings")]
    [SerializeField] private Text senderText; // Text elements of the chat message.
    [SerializeField] private Text messageText;

    //[SyncVar] public NetworkInstanceId chatNetId; // Network ID of the chat box.

    public NetworkVariable<string> sender; // The sender 
    public NetworkVariable<string> message;

    //void Start()
    //{
    //    GameObject chatParent = ClientScene.FindLocalObject(chatNetId);
    //    transform.SetParent(chatParent.transform);
    //    senderText.text = $"[{sender}]:";
    //    messageText.text = message.Value;
    //}

    public void Setup(string sender, string message)
    {
        senderText.text = string.Format("[{0}]:", sender);
        messageText.text = message;
        this.sender.Value = sender;
        this.message.Value = message;
    }
}
