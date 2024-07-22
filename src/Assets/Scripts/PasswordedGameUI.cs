using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PasswordedGameUI : MonoBehaviour, IAnimationStateHandler
{
    private static readonly int collapseStateHash = Animator.StringToHash("Collapse");
    private static readonly int growTriggerHash = Animator.StringToHash("Grow");
    private static readonly int collapseTriggerHash = Animator.StringToHash("Collapse");

    private Animator animator;
    private Lobby lobby;

    public event Action OnClosed;

    [SerializeField] private InputField passwordField;
    [SerializeField] private Text statusText;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        animator = GetComponent<Animator>();

        joinButton.onClick.AddListener(Join);
        closeButton.onClick.AddListener(Close);
    }

    private void Start()
    {
        Hide();
    }

    public void Show(Lobby lobby)
    {
        this.lobby = lobby;
        animator.SetTrigger(growTriggerHash);
        gameObject.SetActive(true);
    }

    private async void Join()
    {
        string password = passwordField.text;
        if (string.IsNullOrWhiteSpace(password))
        {
            AudioManager.Instance.PlaySound("error_message");
            statusText.text = "The field 'Match Password' is required.";
            return;
        }

        joinButton.enabled = false;

        AudioManager.Instance.PlaySound("button_press");

        if (lobby == null)
        {
            Close();
            return;
        }

        bool joined = await LobbyManager.Instance.TryJoinLobby(lobby, new JoinLobbyByIdOptions { Password = passwordField.text });
        if (!joined)
        {
            AudioManager.Instance.PlaySound("connection_error");
            Close();
        }
    }

    private void Close()
    {
        animator.SetTrigger(collapseTriggerHash);
    }

    private void Hide()
    {
        statusText.text = "";
        joinButton.enabled = true;
        gameObject.SetActive(false);

        OnClosed?.Invoke();
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
