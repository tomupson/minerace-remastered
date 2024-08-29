using System;
using MineRace.Audio;
using MineRace.Authentication;
using MineRace.ConnectionManagement;
using MineRace.UGS;
using MineRace.Utils.Animation;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class PasswordedGameUI : MonoBehaviour, IAnimationStateHandler
{
    private static readonly int collapseStateHash = Animator.StringToHash("Collapse");
    private static readonly int growTriggerHash = Animator.StringToHash("Grow");
    private static readonly int collapseTriggerHash = Animator.StringToHash("Collapse");

    [Inject] private readonly ConnectionManager connectionManager;
    [Inject] private readonly LobbyManager lobbyManager;
    [Inject] private readonly UserAccountManager userAccountManager;

    private Animator animator;
    private Lobby lobby;

    public event Action OnClosed;

    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button joinButton;
    [SerializeField] private Button closeButton;

    [SerializeField] private Sound errorMessageSound;
    [SerializeField] private Sound buttonPressSound;
    [SerializeField] private Sound connectionErrorSound;

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

    private void OnEnable()
    {
        statusText.text = "";
        joinButton.enabled = true;
    }

    public void Open(Lobby lobby)
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
            AudioManager.PlayOneShot(errorMessageSound);
            statusText.text = "The field 'Password' is required.";
            return;
        }

        joinButton.enabled = false;

        AudioManager.PlayOneShot(buttonPressSound);

        if (lobby == null)
        {
            Close();
            return;
        }

        bool joined = await lobbyManager.TryJoinLobby(lobby, new JoinLobbyByIdOptions { Password = passwordField.text });

        joinButton.enabled = true;

        if (joined)
        {
            connectionManager.StartHost(userAccountManager.UserInfo.Username);
            return;
        }

        AudioManager.PlayOneShot(connectionErrorSound);
    }

    private void Close()
    {
        animator.SetTrigger(collapseTriggerHash);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }

    public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Assert(this.animator == animator);
        if (stateInfo.shortNameHash == collapseStateHash)
        {
            Hide();
            OnClosed?.Invoke();
        }
    }
}
