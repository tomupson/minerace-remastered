using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance { get; private set; }

    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;

    public float messageExpireTime = 5;

    private void Awake()
    {
        Instance = this;
    }

    public void ChatSendMessage(string sender, string message)
    {
        if (IsServer)
        {
            ChatSendMessageServerRpc(sender, message);
        }
    }

    [ServerRpc]
    private void ChatSendMessageServerRpc(string sender, string message)
    {
        GameObject chatItem = Instantiate(chatItemPrefab);
        chatItem.GetComponent<NetworkObject>().Spawn(destroyWithScene: true);
        chatItem.transform.SetParent(chat.transform);

        ChatItem messageSettings = chatItem.GetComponent<ChatItem>();
        messageSettings.Setup(sender, message);
        StartCoroutine(WaitForExpire(chatItem));
    }

    private IEnumerator WaitForExpire(GameObject chatItem)
    {
        yield return new WaitForSeconds(messageExpireTime);
        DestroyChatMessageServerRpc(chatItem);
    }

    [ServerRpc]
    private void DestroyChatMessageServerRpc(NetworkObjectReference reference)
    {
        DestroyChatMessageClientRpc(reference);
    }

    [ClientRpc]
    public void DestroyChatMessageClientRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            Destroy(networkObject);
        }
    }
}
