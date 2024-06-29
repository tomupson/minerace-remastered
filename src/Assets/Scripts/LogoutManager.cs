using UnityEngine;
using UnityEngine.SceneManagement;

public class LogoutManager : MonoBehaviour
{
    public void Logout()
    {
        // TODO: Logout

        SceneManager.LoadScene("Login");
    }
}