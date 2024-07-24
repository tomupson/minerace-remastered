using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseManager : MonoBehaviour
{
    private PlayerInputActions inputActions;

    public static PauseManager Instance { get; private set; }

    public event Action<bool> OnPauseStateChanged;

    public bool IsPaused { get; private set; }

    private void Awake()
    {
        Instance = this;

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
        OnPauseStateChanged?.Invoke(paused);
    }
}
