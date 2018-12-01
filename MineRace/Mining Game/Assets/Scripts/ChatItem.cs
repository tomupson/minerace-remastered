using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Networking;

public class ChatItem : NetworkBehaviour
{
    [Header("Message Settings")]
    [SerializeField] private Text senderText; // Text elements of the chat message.
    [SerializeField] private Text messageText;

    [SyncVar] public NetworkInstanceId chatNetId; // Network ID of the chat box.

    [SyncVar] public string sender; // The sender 
    [SyncVar] public string message;

    void Start()
    {
        GameObject chatParent = ClientScene.FindLocalObject(chatNetId);
        transform.SetParent(chatParent.transform);
        senderText.text = string.Format("[{0}]:", sender);
        messageText.text = message;
    }

    public void Setup(string sender, string message)
    {
        senderText.text = string.Format("[{0}]:", sender);
        messageText.text = message;
        this.sender = sender;
        this.message = message;
    }
}
