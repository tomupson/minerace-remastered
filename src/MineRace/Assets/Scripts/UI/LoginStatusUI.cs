using MineRace.Authentication;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class LoginStatusUI : MonoBehaviour
{
    [Inject] private readonly UserAccountManager userAccountManager;

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
            userAccountManager.Logout();
            SceneManager.LoadScene("Login");
        });
    }

    private void Start()
    {
        loginNameText.text = $"LOGGED IN AS: {userAccountManager.UserInfo.Username}";
    }
}
