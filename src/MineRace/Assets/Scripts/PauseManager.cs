using MineRace.Infrastructure;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class PauseManager : MonoBehaviour
{
    [Inject] private readonly IPublisher<PauseStateChangedMessage> pauseStatePublisher;

    private PlayerInputActions inputActions;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.Player.Enable();
        inputActions.Player.PauseUnpause.performed += OnPauseUnpausePerformed;
    }

    private void OnDestroy()
    {
        inputActions.Player.PauseUnpause.performed -= OnPauseUnpausePerformed;
        inputActions.Dispose();
    }

    public void Unpause()
    {
        SetPauseState(paused: false);
    }

    private void OnPauseUnpausePerformed(InputAction.CallbackContext context)
    {
        SetPauseState(!IsPaused);
    }

    private void SetPauseState(bool paused)
    {
        if (IsPaused == paused)
        {
            return;
        }

        IsPaused = paused;
        pauseStatePublisher.Publish(new PauseStateChangedMessage(paused));
    }
}
