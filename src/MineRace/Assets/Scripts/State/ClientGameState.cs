using MineRace.Infrastructure;
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
        builder.RegisterComponent(chatManager);
        builder.RegisterComponent(pauseManager);

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
