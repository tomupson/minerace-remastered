using MineRace.ConnectionManagement;
using MineRace.UGS;
using UnityEngine;
using UnityEngine.UI;

public class CreateMatchUI : MonoBehaviour, IAnimationStateHandler
{
    private static readonly int collapseStateHash = Animator.StringToHash("Collapse");
    private static readonly int growTriggerHash = Animator.StringToHash("Grow");
    private static readonly int collapseTriggerHash = Animator.StringToHash("Collapse");

    private Animator animator;

    [SerializeField] private InputField nameField;
    [SerializeField] private InputField passwordField;
    [SerializeField] private Text statusText;
    [SerializeField] private Button createButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        nameField.onValueChanged.AddListener(OnGameNameChanged);

        createButton.onClick.AddListener(CreateGame);
        closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        Hide();
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
            AudioManager.Instance.PlaySound("error_message");
            statusText.text = "The field 'Game Name' is required.";
            return;
        }

        createButton.enabled = false;

        AudioManager.Instance.PlaySound("button_press");

        bool created = await LobbyManager.Instance.TryCreateLobby(gameName, gamePassword);
        if (created)
        {
            ConnectionManager.Instance.StartHost();
        }
    }

    private void Close()
    {
        animator.SetTrigger(collapseTriggerHash);
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
