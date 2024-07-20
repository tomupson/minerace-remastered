using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginStatusUI : MonoBehaviour
{
    [SerializeField] private Text loginNameText;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button logoutButton;

    private void Awake()
    {
        profileButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene("Profile");
        });

        logoutButton.onClick.AddListener(() =>
        {
            // TODO: Handle logout
            SceneManager.LoadScene("Login");
        });
    }

    private void Start()
    {
        loginNameText.text = $"LOGGED IN AS: {UserAccountManager.Instance.UserInfo.Username}";
    }
}
