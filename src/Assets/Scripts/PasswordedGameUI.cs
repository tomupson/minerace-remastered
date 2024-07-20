using System;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PasswordedGameUI : MonoBehaviour
{
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

        AnimationEventHandler animationEventHandler = GetComponent<AnimationEventHandler>();
        animationEventHandler.OnAnimationComplete += () => Hide();

        joinButton.onClick.AddListener(Join);
        closeButton.onClick.AddListener(Close);

        Hide();
    }

    public void Show(Lobby lobby)
    {
        this.lobby = lobby;
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
            statusText.text = "Failed to join lobby.";
            AudioManager.Instance.PlaySound("connection_error");
            Close();
        }
    }

    private void Close()
    {
        animator.SetTrigger("Collapse");
    }

    private void Hide()
    {
        statusText.text = "";
        joinButton.enabled = true;
        gameObject.SetActive(false);

        OnClosed?.Invoke();
    }
}
