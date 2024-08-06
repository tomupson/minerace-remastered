using System.Collections;
using MineRace.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ChatManager : NetworkBehaviour
{
    [Inject] private readonly PlayerInputReader inputReader;

    [SerializeField] private GameObject chat;
    [SerializeField] private GameObject chatItemPrefab;
    [SerializeField] private float messageExpireTime = 5;

    private void Start()
    {
        inputReader.OnSendChatHook += OnSendChatPerformed;
    }

    public override void OnDestroy()
    {
        inputReader.OnSendChatHook -= OnSendChatPerformed;
        base.OnDestroy();
    }

    public void SendChatMessage(string message)
    {
        SendChatMessageServerRpc(message);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        SessionPlayerData? playerData = SessionManager.Instance.GetPlayerData(rpcParams.Receive.SenderClientId);
        if (!playerData.HasValue)
        {
            return;
        }

        GameObject chatItemObject = Instantiate(chatItemPrefab);

        NetworkObject chatItemNetworkObject = chatItemObject.GetComponent<NetworkObject>();
        chatItemNetworkObject.Spawn(destroyWithScene: true);

        chatItemNetworkObject.TrySetParent(chat, worldPositionStays: false);

        ChatItemUI chatItem = chatItemObject.GetComponent<ChatItemUI>();
        chatItem.Setup(playerData.Value.PlayerName, message);
        StartCoroutine(WaitForExpire(chatItemNetworkObject));
    }

    private IEnumerator WaitForExpire(NetworkObject chatItemNetworkObject)
    {
        yield return new WaitForSeconds(messageExpireTime);
        chatItemNetworkObject.Despawn();
    }

    private void OnSendChatPerformed() =>
        SendChatMessage("This is a chat message");
}
