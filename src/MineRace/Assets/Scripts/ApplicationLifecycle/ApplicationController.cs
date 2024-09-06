using System;
using System.Collections;
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
        private LobbyManager lobbyManager;

        [SerializeField] private ConnectionManager connectionManager;
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private UserAccountManager userAccountManager;

        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);
            builder.RegisterComponent(connectionManager);
            builder.RegisterComponent(networkManager);
            builder.RegisterComponent(userAccountManager);
            builder.RegisterComponent(new NetworkedMessageChannel<NetworkConnectionEventMessage>()).AsImplementedInterfaces();

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
            lobbyManager = Container.Resolve<LobbyManager>();

            subscriptions = new DisposableGroup();

            ISubscriber<QuitApplicationMessage> applicationQuitSubscriber = Container.Resolve<ISubscriber<QuitApplicationMessage>>();
            subscriptions.Add(applicationQuitSubscriber.Subscribe(OnQuitGame));

            Application.wantsToQuit += OnWantsToQuit;

            SceneManager.LoadScene("Menu");
        }

        protected override void OnDestroy()
        {
            subscriptions?.Dispose();
            lobbyManager?.EndTracking();
            base.OnDestroy();
        }

        private void OnQuitGame(QuitApplicationMessage msg)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        private bool OnWantsToQuit()
        {
            Application.wantsToQuit -= OnWantsToQuit;

            bool canQuit = lobbyManager?.ActiveLobby == null;
            if (!canQuit)
            {
                StartCoroutine(LeaveBeforeQuit());
            }

            return canQuit;
        }

        /// <summary>
        /// In builds, if we are in a lobby and try to send a Leave request on application quit, it won't go through if we're quitting on the same frame.
        /// So, we need to delay just briefly to let the request happen (though we don't need to wait for the result).
        /// </summary>
        private IEnumerator LeaveBeforeQuit()
        {
            try
            {
                lobbyManager.EndTracking();
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            yield return null;
            Application.Quit();
        }
    }
}
