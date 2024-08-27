using MineRace.Infrastructure;
using MineRace.Utils.Netcode;
using Unity.Netcode;
using UnityEngine;
using VContainer;
using VContainer.Unity;

[RequireComponent(typeof(NetworkHooks))]
public class ClientGameState : GameStateBehaviour
{
    [Inject] private readonly NetworkManager networkManager;

    [SerializeField] private ChatManager chatManager;
    [SerializeField] private PauseManager pauseManager;
    [SerializeField] private NetworkGameState networkGameState;

    protected override void Configure(IContainerBuilder builder)
    {
        base.Configure(builder);
        builder.RegisterComponent(new NetworkedMessageChannel<NetworkChatMessage>()).AsImplementedInterfaces();

        builder.RegisterInstance(chatManager);
        builder.RegisterInstance(pauseManager);
        builder.RegisterInstance(networkGameState);
        builder.RegisterInstance(new MessageChannel<PauseStateChangedMessage>()).AsImplementedInterfaces();
    }

    protected override void Awake()
    {
        base.Awake();

        NetworkHooks networkHooks = GetComponent<NetworkHooks>();
        networkHooks.OnNetworkSpawnHook += OnNetworkSpawn;
    }

    private void OnNetworkSpawn()
    {
        if (!networkManager.IsClient)
        {
            enabled = false;
            return;
        }
    }
}
