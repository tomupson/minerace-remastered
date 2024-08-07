using JetBrains.Annotations;
using MineRace.ConnectionManagement;
using MineRace.Infrastructure;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class ConnectStatusMessageUI : MonoBehaviour
{
    [SerializeField] private Text messageText;
    [SerializeField] private Button menuButton;

    private DisposableGroup subscriptions;

    [Inject, UsedImplicitly]
    private void InjectDependencies(ISubscriber<ConnectStatus> connectStatusSubscriber)
    {
        subscriptions = new DisposableGroup();
        subscriptions.Add(connectStatusSubscriber.Subscribe(OnConnectStatus));
    }

    private void Awake()
    {
        menuButton.onClick.AddListener(ReturnToMenu);
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        subscriptions?.Dispose();
    }

    private void OnConnectStatus(ConnectStatus status)
    {
        switch (status)
        {
            case ConnectStatus.GenericDisconnect:
                Show("The connection to the host was lost.");
                break;
            case ConnectStatus.HostEndedSession:
                Show("The host has ended the game session.");
                break;
        }
    }

    private void Show(string message)
    {
        messageText.text = message;
        gameObject.SetActive(true);
    }

    private void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }
}