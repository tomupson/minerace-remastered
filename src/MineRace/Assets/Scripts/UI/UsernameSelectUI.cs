using MineRace.Authentication;
using MineRace.Utils;
using MineRace.Utils.Animation;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class UsernameSelectUI : MonoBehaviour, IAnimationStateHandler
{
    private const ushort UsernameMaxLength = 64;

    private static readonly int collapseStateHash = Animator.StringToHash("Collapse");
    private static readonly int growTriggerHash = Animator.StringToHash("Grow");
    private static readonly int collapseTriggerHash = Animator.StringToHash("Collapse");

    [Inject] private readonly UserAccountManager userAccountManager;

    [SerializeField] private Animator animator;
    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private TextMeshProUGUI usernameStatusText;

    private void Awake()
    {
        saveButton.onClick.AddListener(Save);

        usernameInputField.onValueChanged.AddListener(OnUsernameChanged);
    }

    private void Start()
    {
        usernameStatusText.text = "";

        Hide();
    }

    public void Open()
    {
        animator.SetTrigger(growTriggerHash);
        gameObject.SetActive(true);
    }

    private void Close()
    {
        animator.SetTrigger(collapseTriggerHash);
    }

    private void Save()
    {
        usernameStatusText.text = "";

        string username = usernameInputField.text;
        if (string.IsNullOrWhiteSpace(username))
        {
            usernameStatusText.text = "Invalid username";
            return;
        }

        if (username.Length > UsernameMaxLength)
        {
            usernameStatusText.text = $"Username cannot be longer than {UsernameMaxLength} characters";
            return;
        }

        userAccountManager.SetUsername(username);
        ClientPrefs.SetPlayerName(username);
        Close();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnUsernameChanged(string username)
    {
        usernameStatusText.text = "";
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
