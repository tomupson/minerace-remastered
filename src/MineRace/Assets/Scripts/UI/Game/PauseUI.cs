using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PauseUI : MonoBehaviour
{
    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly PauseManager pauseManager;

    private DisposableGroup subscriptions;

    [SerializeField] private Button resumeButton;
    [SerializeField] private Button leaveButton;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<PauseStateChangedMessage> pauseStateSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(pauseStateSubscriber.Subscribe(OnPauseStateChanged));
    }

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => pauseManager.Unpause());
        leaveButton.onClick.AddListener(() => connectionManager.RequestShutdown());
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnPauseStateChanged(PauseStateChangedMessage message)
    {
        gameObject.SetActive(message.IsPaused);
    }
}
