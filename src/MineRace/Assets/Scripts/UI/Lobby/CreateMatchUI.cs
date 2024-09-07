using MineRace.Audio;
using MineRace.Authentication;
using MineRace.ConnectionManagement;
using MineRace.UGS;
using MineRace.Utils.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class CreateMatchUI : MonoBehaviour, IAnimationStateHandler
{
    private static readonly int collapseStateHash = Animator.StringToHash("Collapse");
    private static readonly int growTriggerHash = Animator.StringToHash("Grow");
    private static readonly int collapseTriggerHash = Animator.StringToHash("Collapse");

    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly LobbyManager lobbyManager;
    [Inject] private readonly UserAccountManager userAccountManager;

    private Animator animator;

    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button createButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Sound errorMessageSound;
    [SerializeField] private Sound buttonPressSound;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        nameField.onValueChanged.AddListener(OnGameNameChanged);

        createButton.onClick.AddListener(CreateGame);
        closeButton.onClick.AddListener(Close);

        Hide();
    }

    private void OnEnable()
    {
        nameField.text = "";
        passwordField.text = "";
        statusText.text = "";
        createButton.enabled = true;
    }

    public void Open()
    {
        gameObject.SetActive(true);
        animator.SetTrigger(growTriggerHash);
    }

    private async void CreateGame()
    {
        string gameName = nameField.text;
        string gamePassword = passwordField.text;

        if (string.IsNullOrWhiteSpace(gameName))
        {
            AudioManager.PlayOneShot(errorMessageSound);
            statusText.text = "The field 'Game Name' is required.";
            return;
        }

        createButton.enabled = false;

        AudioManager.PlayOneShot(buttonPressSound);

        bool created = await lobbyManager.TryCreateLobby(gameName, gamePassword, connectionManager.MaxConnectedPlayers);
        if (created)
        {
            connectionManager.StartHost(userAccountManager.UserInfo.Username);
            return;
        }

        createButton.enabled = true;
    }

    private void Close()
    {
        animator.SetTrigger(collapseTriggerHash);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnGameNameChanged(string gameName)
    {
        statusText.text = "";
    }

    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Assert(this.animator == animator);
        if (stateInfo.shortNameHash == collapseStateHash)
        {
            Hide();
        }
    }
}
