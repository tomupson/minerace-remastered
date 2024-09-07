using MineRace.ApplicationLifecycle.Messages;
using MineRace.Authentication;
using MineRace.Infrastructure;
using MineRace.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class LoginUI : MonoBehaviour
{
    [Inject] private readonly IPublisher<QuitApplicationMessage> applicationQuitPublisher;
    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private TextMeshProUGUI loginStatusText;
    [SerializeField] private Button registerButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(Login);
        exitButton.onClick.AddListener(() => applicationQuitPublisher.Publish(new QuitApplicationMessage()));
        registerButton.onClick.AddListener(() => Application.OpenURL("https://tomupson.com/minerace/register"));

        usernameInputField.onValueChanged.AddListener(OnInputChanged);
        passwordInputField.onValueChanged.AddListener(OnInputChanged);

        loginStatusText.text = "";
    }

    private void Start()
    {
        // Until proper login is implemented, login on start
        Login();
    }

    private async void Login()
    {
        loginStatusText.text = "";

        string username = ClientPrefs.GetPlayerName();
        //string username = usernameInputField.text;
        //if (string.IsNullOrWhiteSpace(username))
        //{
        //    loginStatusText.text = "The field 'Username' is required.";
        //    return;
        //}

        loginButton.enabled = false;
        loginStatusText.text = "Logging you in. Please wait...";

        bool loggedIn = await userAccountManager.Login(username);
        if (!loggedIn)
        {
            loginButton.enabled = true;
            loginStatusText.text = "Failed to log in.";
            return;
        }

        SceneManager.LoadScene("Menu");
    }

    private void OnInputChanged(string value)
    {
        loginStatusText.text = "";
    }
}
