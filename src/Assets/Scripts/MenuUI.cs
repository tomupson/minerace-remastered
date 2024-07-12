using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Text loginNameText;
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private Button profileButton;
    [SerializeField] private Button logoutButton;

    private void Awake()
    {
        loginNameText.text = $"LOGGED IN AS: {UserAccountManager.Instance.UserInfo.Username}";

        playButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            SceneManager.LoadScene("Lobby");
        });

        exitButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            Application.Quit();
        });

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
}
