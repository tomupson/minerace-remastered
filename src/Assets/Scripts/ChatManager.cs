// TODO: NETWORKING
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;
    [HideInInspector] public List<ChatItem> messages;

    public float messageExpireTime = 5;

    public static ChatManager Instance;

    void Awake()
    {
        Instance = this;
    }

    public void ChatSendMessage(string sender, string message)
    {
        //if (IsServer)
        //    CmdChatSendMessage(sender, message);
    }

    //[Command]
    //void CmdChatSendMessage(string sender, string message)
    //{
    //    GameObject chatItem = Instantiate(chatItemPrefab);
    //    chatItem.transform.SetParent(chat.transform);
    //    ChatItem messageSettings = chatItem.GetComponent<ChatItem>();
    //    messageSettings.Setup(sender, message);
    //    messageSettings.chatNetId = chat.GetComponent<NetworkIdentity>().netId;
    //    NetworkServer.Spawn(chatItem);
    //    StartCoroutine(WaitForExpire(chatItem));
    //}

    public IEnumerator WaitForExpire(GameObject chatItem)
    {
        yield return new WaitForSeconds(messageExpireTime);
        //CmdDestroyChatMessage(chatItem);
    }

    //[Command]
    //void CmdDestroyChatMessage(GameObject chatItem)
    //{
    //    NetworkServer.Destroy(chatItem);
    //}
}
