using MineRace.Authentication;
using MineRace.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class LoginStatusUI : MonoBehaviour
{
    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private TextMeshProUGUI loginNameText;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button logoutButton;

    private void Awake()
    {
        optionsButton.onClick.AddListener(() =>
        {
            // TODO
        });

        logoutButton.onClick.AddListener(() =>
        {
            ClientPrefs.ClearPlayerName();
            userAccountManager.Logout();
        });

        userAccountManager.OnUsernameChanged += OnUsernameChanged;
        if (userAccountManager.UserInfo != null)
        {
            OnUsernameChanged(userAccountManager.UserInfo.Username);
        }
    }

    private void OnDestroy()
    {
        userAccountManager.OnUsernameChanged -= OnUsernameChanged;
    }

    private void OnUsernameChanged(string username)
    {
        loginNameText.text = $"LOGGED IN AS: {username}";
    }
}
