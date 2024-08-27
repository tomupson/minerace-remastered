//------------------------------------------------------------------------------
// <auto-generated>
//     This code was auto-generated by com.unity.inputsystem:InputActionCodeGenerator
//     version 1.7.0
//     from Assets/ScriptableObjects/Input/PlayerInputActions.inputactions
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
            ""name"": ""Game"",
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
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""cabab5ef-837f-46f4-bf34-b033c531937b"",
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
                },
                {
                    ""name"": ""Pause"",
                    ""type"": ""Button"",
                    ""id"": ""84f306fe-c434-4c73-854d-59013061cb24"",
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
                    ""name"": ""SendChat"",
                    ""type"": ""Button"",
                    ""id"": ""f7dbc9fc-5b35-4cf1-b06e-e75599643307"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleChat"",
                    ""type"": ""Button"",
                    ""id"": ""d92f00fc-29ea-4d14-a10b-3534205a8d9a"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                },
                {
                    ""name"": ""ToggleNetStats"",
                    ""type"": ""Button"",
                    ""id"": ""70e99494-d49f-40ed-b745-cdb5e8d5dbaa"",
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
                    ""id"": ""93f2e35e-9af1-457d-a277-7675854df4cc"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Pause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""e06bac9a-cfe0-47b2-a086-f7073e1ada20"",
                    ""path"": ""<Keyboard>/f1"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleChat"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b366943-38eb-46ba-883e-2571c041461c"",
                    ""path"": ""<Keyboard>/f3"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ToggleNetStats"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""b1418690-55d6-4d86-98bb-136321bb1b44"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
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
        },
        {
            ""name"": ""Pause"",
            ""id"": ""87de5e16-3f15-4afd-92f2-d294579cf65d"",
            ""actions"": [
                {
                    ""name"": ""Unpause"",
                    ""type"": ""Button"",
                    ""id"": ""ce657145-f362-4110-a15c-ef02a7610f5f"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """",
                    ""initialStateCheck"": false
                }
            ],
            ""bindings"": [
                {
                    ""name"": """",
                    ""id"": ""65cfb57e-85bd-4dd6-8e7a-50885260d73e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Unpause"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Game
        m_Game = asset.FindActionMap("Game", throwIfNotFound: true);
        m_Game_Move = m_Game.FindAction("Move", throwIfNotFound: true);
        m_Game_Jump = m_Game.FindAction("Jump", throwIfNotFound: true);
        m_Game_Mine = m_Game.FindAction("Mine", throwIfNotFound: true);
        m_Game_Pause = m_Game.FindAction("Pause", throwIfNotFound: true);
        m_Game_Spectate = m_Game.FindAction("Spectate", throwIfNotFound: true);
        m_Game_SendChat = m_Game.FindAction("SendChat", throwIfNotFound: true);
        m_Game_ToggleChat = m_Game.FindAction("ToggleChat", throwIfNotFound: true);
        m_Game_ToggleNetStats = m_Game.FindAction("ToggleNetStats", throwIfNotFound: true);
        // Pause
        m_Pause = asset.FindActionMap("Pause", throwIfNotFound: true);
        m_Pause_Unpause = m_Pause.FindAction("Unpause", throwIfNotFound: true);
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

    // Game
    private readonly InputActionMap m_Game;
    private List<IGameActions> m_GameActionsCallbackInterfaces = new List<IGameActions>();
    private readonly InputAction m_Game_Move;
    private readonly InputAction m_Game_Jump;
    private readonly InputAction m_Game_Mine;
    private readonly InputAction m_Game_Pause;
    private readonly InputAction m_Game_Spectate;
    private readonly InputAction m_Game_SendChat;
    private readonly InputAction m_Game_ToggleChat;
    private readonly InputAction m_Game_ToggleNetStats;
    public struct GameActions
    {
        private @PlayerInputActions m_Wrapper;
        public GameActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move => m_Wrapper.m_Game_Move;
        public InputAction @Jump => m_Wrapper.m_Game_Jump;
        public InputAction @Mine => m_Wrapper.m_Game_Mine;
        public InputAction @Pause => m_Wrapper.m_Game_Pause;
        public InputAction @Spectate => m_Wrapper.m_Game_Spectate;
        public InputAction @SendChat => m_Wrapper.m_Game_SendChat;
        public InputAction @ToggleChat => m_Wrapper.m_Game_ToggleChat;
        public InputAction @ToggleNetStats => m_Wrapper.m_Game_ToggleNetStats;
        public InputActionMap Get() { return m_Wrapper.m_Game; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(GameActions set) { return set.Get(); }
        public void AddCallbacks(IGameActions instance)
        {
            if (instance == null || m_Wrapper.m_GameActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_GameActionsCallbackInterfaces.Add(instance);
            @Move.started += instance.OnMove;
            @Move.performed += instance.OnMove;
            @Move.canceled += instance.OnMove;
            @Jump.started += instance.OnJump;
            @Jump.performed += instance.OnJump;
            @Jump.canceled += instance.OnJump;
            @Mine.started += instance.OnMine;
            @Mine.performed += instance.OnMine;
            @Mine.canceled += instance.OnMine;
            @Pause.started += instance.OnPause;
            @Pause.performed += instance.OnPause;
            @Pause.canceled += instance.OnPause;
            @Spectate.started += instance.OnSpectate;
            @Spectate.performed += instance.OnSpectate;
            @Spectate.canceled += instance.OnSpectate;
            @SendChat.started += instance.OnSendChat;
            @SendChat.performed += instance.OnSendChat;
            @SendChat.canceled += instance.OnSendChat;
            @ToggleChat.started += instance.OnToggleChat;
            @ToggleChat.performed += instance.OnToggleChat;
            @ToggleChat.canceled += instance.OnToggleChat;
            @ToggleNetStats.started += instance.OnToggleNetStats;
            @ToggleNetStats.performed += instance.OnToggleNetStats;
            @ToggleNetStats.canceled += instance.OnToggleNetStats;
        }

        private void UnregisterCallbacks(IGameActions instance)
        {
            @Move.started -= instance.OnMove;
            @Move.performed -= instance.OnMove;
            @Move.canceled -= instance.OnMove;
            @Jump.started -= instance.OnJump;
            @Jump.performed -= instance.OnJump;
            @Jump.canceled -= instance.OnJump;
            @Mine.started -= instance.OnMine;
            @Mine.performed -= instance.OnMine;
            @Mine.canceled -= instance.OnMine;
            @Pause.started -= instance.OnPause;
            @Pause.performed -= instance.OnPause;
            @Pause.canceled -= instance.OnPause;
            @Spectate.started -= instance.OnSpectate;
            @Spectate.performed -= instance.OnSpectate;
            @Spectate.canceled -= instance.OnSpectate;
            @SendChat.started -= instance.OnSendChat;
            @SendChat.performed -= instance.OnSendChat;
            @SendChat.canceled -= instance.OnSendChat;
            @ToggleChat.started -= instance.OnToggleChat;
            @ToggleChat.performed -= instance.OnToggleChat;
            @ToggleChat.canceled -= instance.OnToggleChat;
            @ToggleNetStats.started -= instance.OnToggleNetStats;
            @ToggleNetStats.performed -= instance.OnToggleNetStats;
            @ToggleNetStats.canceled -= instance.OnToggleNetStats;
        }

        public void RemoveCallbacks(IGameActions instance)
        {
            if (m_Wrapper.m_GameActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IGameActions instance)
        {
            foreach (var item in m_Wrapper.m_GameActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_GameActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public GameActions @Game => new GameActions(this);

    // Pause
    private readonly InputActionMap m_Pause;
    private List<IPauseActions> m_PauseActionsCallbackInterfaces = new List<IPauseActions>();
    private readonly InputAction m_Pause_Unpause;
    public struct PauseActions
    {
        private @PlayerInputActions m_Wrapper;
        public PauseActions(@PlayerInputActions wrapper) { m_Wrapper = wrapper; }
        public InputAction @Unpause => m_Wrapper.m_Pause_Unpause;
        public InputActionMap Get() { return m_Wrapper.m_Pause; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PauseActions set) { return set.Get(); }
        public void AddCallbacks(IPauseActions instance)
        {
            if (instance == null || m_Wrapper.m_PauseActionsCallbackInterfaces.Contains(instance)) return;
            m_Wrapper.m_PauseActionsCallbackInterfaces.Add(instance);
            @Unpause.started += instance.OnUnpause;
            @Unpause.performed += instance.OnUnpause;
            @Unpause.canceled += instance.OnUnpause;
        }

        private void UnregisterCallbacks(IPauseActions instance)
        {
            @Unpause.started -= instance.OnUnpause;
            @Unpause.performed -= instance.OnUnpause;
            @Unpause.canceled -= instance.OnUnpause;
        }

        public void RemoveCallbacks(IPauseActions instance)
        {
            if (m_Wrapper.m_PauseActionsCallbackInterfaces.Remove(instance))
                UnregisterCallbacks(instance);
        }

        public void SetCallbacks(IPauseActions instance)
        {
            foreach (var item in m_Wrapper.m_PauseActionsCallbackInterfaces)
                UnregisterCallbacks(item);
            m_Wrapper.m_PauseActionsCallbackInterfaces.Clear();
            AddCallbacks(instance);
        }
    }
    public PauseActions @Pause => new PauseActions(this);
    public interface IGameActions
    {
        void OnMove(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnMine(InputAction.CallbackContext context);
        void OnPause(InputAction.CallbackContext context);
        void OnSpectate(InputAction.CallbackContext context);
        void OnSendChat(InputAction.CallbackContext context);
        void OnToggleChat(InputAction.CallbackContext context);
        void OnToggleNetStats(InputAction.CallbackContext context);
    }
    public interface IPauseActions
    {
        void OnUnpause(InputAction.CallbackContext context);
    }
}
