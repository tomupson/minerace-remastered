using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Text loginNameText;

    void Update()
    {
        loginNameText.text = "LOGGED IN AS: " + UserAccountManager.instance.userInfo.Username;
    }

    public void PlayGame()
    {
        AudioManager.instance.PlaySound("button_press");
        SceneManager.LoadScene("Lobby");
    }

    public void Options()
    {
        Debug.Log("Options feature not currently available.");
        //SceneManager.LoadScene("Options");
    }

    public void ExitGame()
    {
        AudioManager.instance.PlaySound("button_press");
        Application.Quit();
    }

    public void OpenProfile()
    {
        SceneManager.LoadScene("Profile");
    }
}
