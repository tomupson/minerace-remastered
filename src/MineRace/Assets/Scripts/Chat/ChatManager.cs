using System.Collections;
using MineRace.ConnectionManagement;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Assertions;

public class ChatManager : NetworkBehaviour
{
    private const string ServerSender = "SERVER";

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CanvasGroup chatRootCanvasGroup;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject chatItemPrefab;
    [SerializeField] private float messageExpireTime = 5;

    private void Awake()
    {
        inputReader.OnSendChatHook += OnSendChatPerformed;
        inputReader.OnToggleChatHook += OnToggleChat;

        chatRootCanvasGroup.alpha = 0f;
        chatRootCanvasGroup.blocksRaycasts = false;
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

    public void SendServerChatMessage(string message, Color colour = default)
    {
        Assert.IsTrue(IsServer, "Attempting to send server chat message not as server");
        SpawnChatMessage(ServerSender, message, colour);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        SessionPlayerData? playerData = SessionManager.Instance.GetPlayerData(rpcParams.Receive.SenderClientId);
        if (!playerData.HasValue)
        {
            return;
        }

        SpawnChatMessage(playerData.Value.PlayerName, message);
    }

    private void SpawnChatMessage(string playerName, string message, Color colour = default)
    {
        GameObject chatItemObject = Instantiate(chatItemPrefab);

        NetworkObject chatItemNetworkObject = chatItemObject.GetComponent<NetworkObject>();
        chatItemNetworkObject.Spawn(destroyWithScene: true);

        chatItemNetworkObject.TrySetParent(chatContent, worldPositionStays: false);

        ChatItemUI chatItem = chatItemObject.GetComponent<ChatItemUI>();
        chatItem.Setup(playerName, message, colour);
        StartCoroutine(WaitForExpire(chatItemNetworkObject));
    }

    private IEnumerator WaitForExpire(NetworkObject chatItemNetworkObject)
    {
        yield return new WaitForSeconds(messageExpireTime);
        chatItemNetworkObject.Despawn();
    }

    private void OnSendChatPerformed() =>
        SendChatMessage("This is a chat message");

    private void OnToggleChat()
    {
        int alpha = (int)chatRootCanvasGroup.alpha;
        chatRootCanvasGroup.alpha = alpha ^ 1;
        chatRootCanvasGroup.blocksRaycasts = !chatRootCanvasGroup.blocksRaycasts;
    }
}
