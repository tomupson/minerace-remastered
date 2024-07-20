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
        exitButton.onClick.AddListener(() => Application.Quit());
        registerButton.onClick.AddListener(() => Application.OpenURL("https://tomupson.com/minerace/register"));

        usernameInputField.onValueChanged.AddListener(OnInputChanged);
        passwordInputField.onValueChanged.AddListener(OnInputChanged);
    }

    private void Start()
    {
        loginStatusText.text = "";
#if DEBUG
        Login();
#endif
    }

    private async void Login()
    {
        loginStatusText.text = "";
        bool loggedIn = await UserAccountManager.Instance.Login(usernameInputField.text);
        if (!loggedIn)
        {
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
