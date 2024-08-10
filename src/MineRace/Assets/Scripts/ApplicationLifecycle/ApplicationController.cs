using MineRace.ApplicationLifecycle.Messages;
using MineRace.Authentication;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using MineRace.UGS;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using VContainer.Unity;

namespace MineRace.ApplicationLifecycle
{
    public class ApplicationController : LifetimeScope
    {
        private DisposableGroup subscriptions;

        [SerializeField] private ConnectionManager connectionManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private UserAccountManager userAccountManager;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(connectionManager);
            builder.RegisterComponent(networkManager);
            builder.RegisterComponent(userAccountManager);
            builder.RegisterComponent(new MessageChannel<ConnectionEventMessage>()).AsImplementedInterfaces();

            builder.RegisterInstance(new MessageChannel<QuitApplicationMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<ConnectStatus>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<ReconnectMessage>()).AsImplementedInterfaces();
            builder.RegisterInstance(new MessageChannel<LobbyStatus>()).AsImplementedInterfaces();

            builder.RegisterEntryPoint<LobbyManager>(Lifetime.Singleton).AsSelf();
        }

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            base.Awake();
        }

        private void Start()
        {
            subscriptions = new DisposableGroup();

            ISubscriber<QuitApplicationMessage> applicationQuitSubscriber = Container.Resolve<ISubscriber<QuitApplicationMessage>>();
            subscriptions.Add(applicationQuitSubscriber.Subscribe(QuitGame));

            SceneManager.LoadScene("Login");
        }

        protected override void OnDestroy()
        {
            subscriptions?.Dispose();
            base.OnDestroy();
        }

        private void QuitGame(QuitApplicationMessage msg)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
