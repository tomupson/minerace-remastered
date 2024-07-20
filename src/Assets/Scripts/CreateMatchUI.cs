using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreateMatchUI : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private InputField nameField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Text statusText;
    [SerializeField] private Button createButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        AnimationEventHandler animationEventHandler = GetComponent<AnimationEventHandler>();
        animationEventHandler.OnAnimationComplete += () => Hide();

        nameField.onValueChanged.AddListener(OnGameNameChanged);

        createButton.onClick.AddListener(CreateGame);
        closeButton.onClick.AddListener(Close);

        Hide();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    private async void CreateGame()
    {
        string gameName = nameField.text;
        string gamePassword = passwordField.text;

        if (string.IsNullOrWhiteSpace(gameName))
        {
            AudioManager.Instance.PlaySound("error_message");
            statusText.text = "The field 'Game Name' is required.";
            return;
        }

        createButton.enabled = false;

        AudioManager.Instance.PlaySound("button_press");

        bool created = await LobbyManager.Instance.TryCreateLobby(gameName, gamePassword);
        if (created)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Game", LoadSceneMode.Single);
        }
    }

    private void Close()
    {
        animator.SetTrigger("Collapse");
    }

    private void Hide()
    {
        nameField.text = "";
        passwordField.text = "";
        statusText.text = "";
        createButton.enabled = true;
        gameObject.SetActive(false);
    }

    private void OnGameNameChanged(string gameName)
    {
        statusText.text = "";
    }
}
