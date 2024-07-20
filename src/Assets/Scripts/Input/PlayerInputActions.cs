//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/GameData/PlayerInputActions.inputactions
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public partial class @PlayerInputActions: IInputActionCollection2, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerInputActions()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerInputActions"",
    ""maps"": [
        {
            ""name"": ""Player"",
            ""id"": ""a40606ac-deb9-443b-8d02-42228d94df40"",
            ""actions"": [
                {
                    ""name"": ""Move"",
                    ""type"": ""Value"",
                    ""id"": ""696e94c9-afef-4108-8782-013a9e15174f"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": true
                },
                {
                    ""name"": ""SendChat"",
                    ""type"": ""Button"",
                    ""id"": ""f7dbc9fc-5b35-4cf1-b06e-e75599643307"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""PauseUnpause"",
                    ""type"": ""Button"",
                    ""id"": ""938d17c0-5fe0-4c6a-95ba-1753d17224f9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Spectate"",
                    ""type"": ""Button"",
                    ""id"": ""366cbbab-8ecf-49d0-a4d7-1d28670928e2"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Screenshot"",
                    ""type"": ""Button"",
                    ""id"": ""78723720-7ab3-4332-851d-e87ae0ea10ed"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""Mine"",
                    ""type"": ""Button"",
                    ""id"": ""cb748e2e-3a02-4804-aca8-a224564eb17e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Horizontal"",
                    ""id"": ""f9c4c3e5-d101-4537-8227-fd1fa5735d38"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""c258fb51-d2dc-430b-9844-4649bcd766fb"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""18e4e289-51f1-4ed7-bf96-e63267a9d697"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Move"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""8e413257-9c0d-4730-9a2d-ee21e45bd1c3"",
                    ""path"": ""<Keyboard>/m"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""SendChat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""15ef98a0-1752-47bd-84ac-c49e1ec78087"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""PauseUnpause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""c64800d5-a2ce-4873-880a-592a85a5613b"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Spectate"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""6a7dc2ff-9535-408b-bb5a-5d4e946ae8d3"",
                    ""path"": ""<Keyboard>/f12"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Screenshot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""721149cc-faa4-47ee-97c4-c8edbf16f567"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Mine"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Player
        m_Player = asset.FindActionMap("Player", throwIfNotFound: true);
        m_Player_Move = m_Player.FindAction("Move", throwIfNotFound: true);
        m_Player_SendChat = m_Player.FindAction("SendChat", throwIfNotFound: true);
        m_Player_PauseUnpause = m_Player.FindAction("PauseUnpause", throwIfNotFound: true);
        m_Player_Spectate = m_Player.FindAction("Spectate", throwIfNotFound: true);
        m_Player_Screenshot = m_Player.FindAction("Screenshot", throwIfNotFound: true);
        m_Player_Mine = m_Player.FindAction("Mine", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public IEnumerable<InputBinding> bindings => asset.bindings;

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding bindingMask, out InputAction action)
    {
        return asset.FindBinding(bindingMask, out action);
    }

    // Player
    private readonly InputActionMap m_Player;
    private List<IPlayerActions> m_PlayerActionsCallbackInterfaces = new List<IPlayerActions>();
    private readonly InputAction m_Player_Move;
    private readonly InputAction m_Player_SendChat;
    private readonly InputAction m_Player_PauseUnpause;
    private readonly InputAction m_Player_Spectate;
    private readonly InputAction m_Player_Screenshot;
    private readonly InputAction m_Player_Mine;
    public struct PlayerActions
    {
        private @PlayerInputActions m_Wrapper;
        public PlayerActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Player_Move;
        public InputAction @SendChat => m_Wrapper.m_Player_SendChat;
        public InputAction @PauseUnpause => m_Wrapper.m_Player_PauseUnpause;
        public InputAction @Spectate => m_Wrapper.m_Player_Spectate;
        public InputAction @Screenshot => m_Wrapper.m_Player_Screenshot;
        public InputAction @Mine => m_Wrapper.m_Player_Mine;
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
        public void AddCallbacks(IPlayerActions instance)
        {
            if (instance == null || m_Wrapper.m_PlayerActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @SendChat.started += instance.OnSendChat;
            @SendChat.performed += instance.OnSendChat;
            @SendChat.canceled += instance.OnSendChat;
            @PauseUnpause.started += instance.OnPauseUnpause;
            @PauseUnpause.performed += instance.OnPauseUnpause;
            @PauseUnpause.canceled += instance.OnPauseUnpause;
            @Spectate.started += instance.OnSpectate;
            @Spectate.performed += instance.OnSpectate;
            @Spectate.canceled += instance.OnSpectate;
            @Screenshot.started += instance.OnScreenshot;
            @Screenshot.performed += instance.OnScreenshot;
            @Screenshot.canceled += instance.OnScreenshot;
            @Mine.started += instance.OnMine;
            @Mine.performed += instance.OnMine;
            @Mine.canceled += instance.OnMine;
        }

        private void UnregisterCallbacks(IPlayerActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @SendChat.started -= instance.OnSendChat;
            @SendChat.performed -= instance.OnSendChat;
            @SendChat.canceled -= instance.OnSendChat;
            @PauseUnpause.started -= instance.OnPauseUnpause;
            @PauseUnpause.performed -= instance.OnPauseUnpause;
            @PauseUnpause.canceled -= instance.OnPauseUnpause;
            @Spectate.started -= instance.OnSpectate;
            @Spectate.performed -= instance.OnSpectate;
            @Spectate.canceled -= instance.OnSpectate;
            @Screenshot.started -= instance.OnScreenshot;
            @Screenshot.performed -= instance.OnScreenshot;
            @Screenshot.canceled -= instance.OnScreenshot;
            @Mine.started -= instance.OnMine;
            @Mine.performed -= instance.OnMine;
            @Mine.canceled -= instance.OnMine;
        }

        public void RemoveCallbacks(IPlayerActions instance)
        {
            if (m_Wrapper.m_PlayerActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPlayerActions instance)
        {
            foreach (var item in m_Wrapper.m_PlayerActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PlayerActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PlayerActions @Player => new PlayerActions(this);
    public interface IPlayerActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnSendChat(InputAction.CallbackContext context);
        void OnPauseUnpause(InputAction.CallbackContext context);
        void OnSpectate(InputAction.CallbackContext context);
        void OnScreenshot(InputAction.CallbackContext context);
        void OnMine(InputAction.CallbackContext context);
    }
}
