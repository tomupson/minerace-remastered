using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ChatManager : NetworkBehaviour
{
    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;
    [SerializeField] private float messageExpireTime = 5;

    public void SendMessage(string sender, string message)
    {
        SendChatMessageServerRpc(sender, message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string sender, string message)
    {
        GameObject chatItemObject = Instantiate(chatItemPrefab);

        NetworkObject chatItemNetworkObject = chatItemObject.GetComponent<NetworkObject>();
        chatItemNetworkObject.Spawn(destroyWithScene: true);

        chatItemNetworkObject.TrySetParent(chat, worldPositionStays: false);

        ChatItemUI chatItem = chatItemObject.GetComponent<ChatItemUI>();
        chatItem.Setup(sender, message);
        StartCoroutine(WaitForExpire(chatItemNetworkObject));
    }

    private IEnumerator WaitForExpire(NetworkObject chatItemNetworkObject)
    {
        yield return new WaitForSeconds(messageExpireTime);
        chatItemNetworkObject.Despawn();
    }
}
