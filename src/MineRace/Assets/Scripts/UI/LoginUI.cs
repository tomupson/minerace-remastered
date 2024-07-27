using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private Button loginButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Text loginStatusText;
    [SerializeField] private Button registerButton;

    private void Awake()
    {
        loginButton.onClick.AddListener(Login);
        exitButton.onClick.AddListener(() =>
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
        registerButton.onClick.AddListener(() => Application.OpenURL("https://tomupson.com/minerace/register"));

        usernameInputField.onValueChanged.AddListener(OnInputChanged);
        passwordInputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void Start()
    {
        loginStatusText.text = "";
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        Login();
#endif
    }

    private async void Login()
    {
        loginStatusText.text = "";

        string username = usernameInputField.text;
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        if (string.IsNullOrWhiteSpace(username))
        {
            loginStatusText.text = "The field 'Username' is required.";
            return;
        }
#endif

        loginButton.enabled = false;
        loginStatusText.text = "Logging you in. Please wait...";

        bool loggedIn = await UserAccountManager.Instance.Login(usernameInputField.text);
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
