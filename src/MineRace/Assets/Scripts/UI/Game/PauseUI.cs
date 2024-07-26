using MineRace.ConnectionManagement;
using UnityEngine;
using UnityEngine.UI;

public class PauseUI : MonoBehaviour
{
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button leaveButton;

    private void Awake()
    {
        resumeButton.onClick.AddListener(() => PauseManager.Instance.Unpause());
        leaveButton.onClick.AddListener(() => ConnectionManager.Instance.RequestShutdown());
    }

    private void Start()
    {
        PauseManager.Instance.OnPauseStateChanged += OnPauseStateChanged;

        OnPauseStateChanged(PauseManager.Instance.IsPaused);
    }

    private void OnPauseStateChanged(bool isPaused)
    {
        gameObject.SetActive(isPaused);
    }
}
