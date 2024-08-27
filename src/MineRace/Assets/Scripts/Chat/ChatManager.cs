using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using Unity.Netcode;
using UnityEngine;
using VContainer;

public class ChatManager : MonoBehaviour
{
    private const string ServerSender = "SERVER";

    [Inject] private readonly IPublisher<NetworkChatMessage> chatMessagePublisher;

    private DisposableGroup subscriptions;

    [SerializeField] private PlayerInputReader inputReader;
    [SerializeField] private CanvasGroup chatRootCanvasGroup;
    [SerializeField] private Transform chatContent;
    [SerializeField] private GameObject chatItemPrefab;
    [SerializeField] private float messageExpireTime = 5;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<NetworkChatMessage> chatMessageSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(chatMessageSubscriber.Subscribe(OnChatMessageReceived));
    }

    private void Awake()
    {
        inputReader.OnSendChatHook += OnSendChatPerformed;
        inputReader.OnToggleChatHook += OnToggleChat;

        chatRootCanvasGroup.alpha = 0f;
        chatRootCanvasGroup.blocksRaycasts = false;
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();

        inputReader.OnSendChatHook -= OnSendChatPerformed;
        inputReader.OnToggleChatHook -= OnToggleChat;
    }

    public void SendChatMessage(string message)
    {
        SendChatMessageServerRpc(message);
    }

    public void SpawnServerChatMessage(string message, Color colour = default)
    {
        SpawnChatMessage(ServerSender, message, colour);
    }

    private void OnChatMessageReceived(NetworkChatMessage message)
    {
        SpawnChatMessage(message.sender.ToString(), message.message.ToString(), message.colour);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendChatMessageServerRpc(string message, ServerRpcParams rpcParams = default)
    {
        SessionPlayerData? sessionPlayerData = SessionManager.Instance.GetPlayerData(rpcParams.Receive.SenderClientId);
        if (!sessionPlayerData.HasValue)
        {
            return;
        }

        chatMessagePublisher.Publish(new NetworkChatMessage(sessionPlayerData.Value.PlayerName, message, Color.white));
    }

    private void SpawnChatMessage(string playerName, string message, Color colour = default)
    {
        GameObject chatItemObject = Instantiate(chatItemPrefab, chatContent);
        ChatItemUI chatItem = chatItemObject.GetComponent<ChatItemUI>();
        chatItem.Setup(playerName, message, colour);
        Destroy(chatItemObject, messageExpireTime);
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
