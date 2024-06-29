using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private InputField usernameInputField;
    [SerializeField] private InputField passwordInputField;

    [Header("Buttons")]
    [SerializeField] private Button loginButton;

    [Header("Status Texts")]
    [SerializeField] private Text loginStatusText;

    [Header("Toggles")]
    [SerializeField] private Toggle rememberMeToggle;

    void Start()
    {
        loginButton.enabled = true;
        loginStatusText.text = "";

        #if UNITY_WEBGL
        rememberMeToggle.isOn = false;
        rememberMeToggle.enabled = false;
        rememberMeToggle.transform.Find("Background").GetComponent<Image>().color = Color.grey;
        #endif

        SceneManager.LoadScene("Menu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
