using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    public static ChatManager Instance { get; private set; }

    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;
    [SerializeField] private float messageExpireTime = 5;

    private void Awake()
    {
        Instance = this;
    }

    public void SendMessage(string sender, string message)
    {
        SendChatMessageServerRpc(sender, message);
    }

    [ServerRpc]
    private void SendChatMessageServerRpc(string sender, string message)
    {
        GameObject chatItemObject = Instantiate(chatItemPrefab);

        NetworkObject chatItemNetworkObject = chatItemObject.GetComponent<NetworkObject>();
        chatItemNetworkObject.Spawn(destroyWithScene: true);

        SetChatMessageParentClientRpc(chatItemObject);

        ChatItem chatItem = chatItemObject.GetComponent<ChatItem>();
        chatItem.Setup(sender, message);
        StartCoroutine(WaitForExpire(chatItemNetworkObject));
    }

    [ClientRpc]
    private void SetChatMessageParentClientRpc(NetworkObjectReference reference)
    {
        if (reference.TryGet(out NetworkObject networkObject))
        {
            networkObject.transform.SetParent(chat.transform);
        }
    }

    private IEnumerator WaitForExpire(NetworkObject chatItemNetworkObject)
    {
        yield return new WaitForSeconds(messageExpireTime);
        chatItemNetworkObject.Despawn();
    }
}
