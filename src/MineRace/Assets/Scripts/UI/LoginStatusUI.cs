using MineRace.Authentication;
using MineRace.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;

public class LoginStatusUI : MonoBehaviour
{
    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private TextMeshProUGUI loginNameText;
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
            ClientPrefs.ClearPlayerName();
            userAccountManager.Logout();
            SceneManager.LoadScene("Login");
        });
    }

    private void Start()
    {
        userAccountManager.OnUsernameChanged += OnUsernameChanged;
        OnUsernameChanged(userAccountManager.UserInfo.Username);
    }

    private void OnUsernameChanged(string username)
    {
        loginNameText.text = $"LOGGED IN AS: {username}";
    }
}
