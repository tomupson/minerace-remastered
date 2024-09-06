using MineRace.ApplicationLifecycle.Messages;
using MineRace.Authentication;
using MineRace.Infrastructure;
using MineRace.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class MenuUI : MonoBehaviour
{
    [Inject] private readonly IPublisher<QuitApplicationMessage> applicationQuitPublisher;
    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private UsernameSelectUI usernameSelectPopup;

    private void Awake()
    {
        playButton.onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
        exitButton.onClick.AddListener(() => applicationQuitPublisher.Publish(new QuitApplicationMessage()));

        userAccountManager.OnLogout += OnLogout;
    }

    private async void Start()
    {
        string username = ClientPrefs.GetPlayerName();
        if (string.IsNullOrEmpty(username))
        {
            usernameSelectPopup.Open();
        }

        await userAccountManager.Login(username);
    }

    private void OnDestroy()
    {
        userAccountManager.OnLogout -= OnLogout;
    }

    private void OnLogout()
    {
        usernameSelectPopup.Open();
    }
}
