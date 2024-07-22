using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button exitButton;

    private void Awake()
    {
        playButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
            SceneManager.LoadScene("Lobby");
        });

        exitButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.PlaySound("button_press");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        });
    }
}
