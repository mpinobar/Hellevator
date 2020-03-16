// GENERATED AUTOMATICALLY FROM 'Assets/GameAssets/Input/InputActionsPruebas.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputActionsPruebas : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputActionsPruebas()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputActionsPruebas"",
    ""maps"": [
        {
            ""name"": ""PruebasAndres"",
            ""id"": ""83628a37-6965-46c8-b22e-9d1a24a01ac5"",
            ""actions"": [
                {
                    ""name"": ""Moverse"",
                    ""type"": ""PassThrough"",
                    ""id"": ""4ec44eaa-21e2-45d4-a78e-bc620afeaabf"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""55c9ef84-7d6d-4868-8f6d-b5b627ebe82c"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""MovementInput"",
                    ""id"": ""f01a64b6-9703-493a-a43d-d76627650e06"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": ""AxisDeadzone"",
                    ""groups"": """",
                    ""action"": ""Moverse"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""6dd019c4-1a04-48f8-8952-343393adf657"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moverse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""206966cf-ffcb-4120-bba2-dff5f2afe91b"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moverse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""71022313-08a7-4127-97cf-b7e4fc2fbd6f"",
                    ""path"": ""<Gamepad>/leftStick/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Moverse"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""17c36550-74a9-436d-9b81-9e58bf1e3b38"",
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
                    ""id"": ""7bdf79b2-ab24-4a28-a431-36063e51892f"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PruebasAndres
        m_PruebasAndres = asset.FindActionMap("PruebasAndres", throwIfNotFound: true);
        m_PruebasAndres_Moverse = m_PruebasAndres.FindAction("Moverse", throwIfNotFound: true);
        m_PruebasAndres_Jump = m_PruebasAndres.FindAction("Jump", throwIfNotFound: true);
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

    // PruebasAndres
    private readonly InputActionMap m_PruebasAndres;
    private IPruebasAndresActions m_PruebasAndresActionsCallbackInterface;
    private readonly InputAction m_PruebasAndres_Moverse;
    private readonly InputAction m_PruebasAndres_Jump;
    public struct PruebasAndresActions
    {
        private @InputActionsPruebas m_Wrapper;
        public PruebasAndresActions(@InputActionsPruebas wrapper) { m_Wrapper = wrapper; }
        public InputAction @Moverse => m_Wrapper.m_PruebasAndres_Moverse;
        public InputAction @Jump => m_Wrapper.m_PruebasAndres_Jump;
        public InputActionMap Get() { return m_Wrapper.m_PruebasAndres; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PruebasAndresActions set) { return set.Get(); }
        public void SetCallbacks(IPruebasAndresActions instance)
        {
            if (m_Wrapper.m_PruebasAndresActionsCallbackInterface != null)
            {
                @Moverse.started -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnMoverse;
                @Moverse.performed -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnMoverse;
                @Moverse.canceled -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnMoverse;
                @Jump.started -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PruebasAndresActionsCallbackInterface.OnJump;
            }
            m_Wrapper.m_PruebasAndresActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Moverse.started += instance.OnMoverse;
                @Moverse.performed += instance.OnMoverse;
                @Moverse.canceled += instance.OnMoverse;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
            }
        }
    }
    public PruebasAndresActions @PruebasAndres => new PruebasAndresActions(this);
    public interface IPruebasAndresActions
    {
        void OnMoverse(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
    }
}
