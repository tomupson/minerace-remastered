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
        loginButton.onClick.AddListener(() =>
        {
            // TODO: Handle login
            SceneManager.LoadScene("Menu");
        });

        exitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        registerButton.onClick.AddListener(() =>
        {
            // TODO: Register
        });
    }

    private void Start()
    {
        loginStatusText.text = "";
    }
}
