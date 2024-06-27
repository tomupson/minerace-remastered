using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;
    [HideInInspector] public List<ChatItem> messages;

    public float messageExpireTime = 5;

    public static ChatManager instance;

    void Awake()
    {
        instance = this;
    }

    public void ChatSendMessage(string sender, string message)
    {
        if (isServer)
            CmdChatSendMessage(sender, message);
    }

    [Command]
    void CmdChatSendMessage(string sender, string message)
    {
        GameObject chatItem = Instantiate(chatItemPrefab);
        chatItem.transform.SetParent(chat.transform);
        ChatItem messageSettings = chatItem.GetComponent<ChatItem>();
        messageSettings.Setup(sender, message);
        messageSettings.chatNetId = chat.GetComponent<NetworkIdentity>().netId;
        NetworkServer.Spawn(chatItem);
        StartCoroutine(WaitForExpire(chatItem));
    }

    public IEnumerator WaitForExpire(GameObject chatItem)
    {
        yield return new WaitForSeconds(messageExpireTime);
        CmdDestroyChatMessage(chatItem);
    }

    [Command]
    void CmdDestroyChatMessage(GameObject chatItem)
    {
        NetworkServer.Destroy(chatItem);
    }
}
