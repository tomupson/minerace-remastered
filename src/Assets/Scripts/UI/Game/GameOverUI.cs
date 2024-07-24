using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    private static readonly int gameOverTriggerHash = Animator.StringToHash("GameOver");
    private Animator gameOverTextAnimator;

    [SerializeField] private Text gameOverText;

    private void Awake()
    {
        gameOverTextAnimator = gameOverText.GetComponent<Animator>();
    }

    private void Start()
    {
        GameManager.Instance.State.OnValueChanged += HandleGameStateChanged;

        gameObject.SetActive(false);
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.Completed && previousState != GameState.Completed)
        {
            gameObject.SetActive(true);
            gameOverText.text = GameManager.Instance.TimeRemaining.Value == 0 ? "TIMES UP!" : "MATCH COMPLETE!";
            gameOverTextAnimator.SetTrigger(gameOverTriggerHash);
        }
    }
}
