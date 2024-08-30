using System;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "InputReader", menuName = "MineRace/Input Reader")]
public class PlayerInputReader : ScriptableObject, PlayerInputActions.IGameActions, PlayerInputActions.IPauseActions
{
    private PlayerInputActions input;

    public event Action<float> OnMoveHook;
    public Action OnJumpHook;
    public Action OnJumpCancelledHook;
    public event Action OnMineHook;
    public event Action OnUseHook;
    public event Action OnSpectateHook;
    public event Action OnSendChatHook;
    public event Action OnPauseHook;
    public event Action OnUnpauseHook;
    public event Action OnToggleChatHook;
    public event Action OnToggleNetStatsHook;

    private void OnEnable()
    {
        if (input == null)
        {
            input = new PlayerInputActions();
            input.Game.SetCallbacks(this);
            input.Pause.SetCallbacks(this);
            SetGameActive();
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        OnMoveHook?.Invoke(context.ReadValue<float>());
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        switch (context.phase)
        {
            case InputActionPhase.Performed:
                OnJumpHook?.Invoke();
                break;
            case InputActionPhase.Canceled:
                OnJumpCancelledHook?.Invoke();
                break;
        }
    }

    public void OnMine(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnMineHook?.Invoke();
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnUseHook?.Invoke();
        }
    }

    public void OnSpectate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSpectateHook?.Invoke();
        }
    }

    public void OnSendChat(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnSendChatHook?.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetPauseActive();
            OnPauseHook?.Invoke();
        }
    }

    public void OnToggleChat(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnToggleChatHook?.Invoke();
        }
    }

    public void OnToggleNetStats(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            OnToggleNetStatsHook?.Invoke();
        }
    }

    public void OnUnpause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            SetGameActive();
            OnUnpauseHook?.Invoke();
        }
    }

    private void SetGameActive()
    {
        input.Game.Enable();
        input.Pause.Disable();
    }

    private void SetPauseActive()
    {
        input.Game.Disable();
        input.Pause.Enable();
    }
}
