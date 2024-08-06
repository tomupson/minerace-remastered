using UnityEngine;
using UnityEngine.UI;
using VContainer;

public class GameOverUI : MonoBehaviour
{
    private static readonly int gameOverTriggerHash = Animator.StringToHash("GameOver");

    [Inject] private readonly NetworkGameState networkGameState;

    private Animator gameOverTextAnimator;

    [SerializeField] private Text gameOverText;

    private void Awake()
    {
        gameOverTextAnimator = gameOverText.GetComponent<Animator>();
    }

    private void Start()
    {
        networkGameState.State.OnValueChanged += HandleGameStateChanged;

        gameObject.SetActive(false);
    }

    private void HandleGameStateChanged(GameState previousState, GameState newState)
    {
        if (newState == GameState.Completed && previousState != GameState.Completed)
        {
            gameObject.SetActive(true);
            gameOverText.text = networkGameState.TimeRemaining.Value == 0 ? "TIMES UP!" : "MATCH COMPLETE!";
            gameOverTextAnimator.SetTrigger(gameOverTriggerHash);
        }
    }
}
