using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;
    [SerializeField] private Toggle rememberMeToggle;
    [SerializeField] private Button loginButton;
    [SerializeField] private Text loginStatusText;

    private void Start()
    {
        loginButton.enabled = true;
        loginStatusText.text = "";

        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
