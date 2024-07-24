using UnityEngine;
using UnityEngine.UI;

public class ReadyUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;

    private void Awake()
    {
        readyButton.onClick.AddListener(() =>
        {
            Hide();
            Player.LocalPlayer.Ready();
        });
    }

    private void Start()
    {
        GameManager.Instance.State.OnValueChanged += HandleGameStateChanged;

        Hide();
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        bool isReadyUp = newState == GameState.WaitingForPlayersReady;
        gameObject.SetActive(isReadyUp);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
