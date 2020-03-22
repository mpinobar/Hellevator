using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{

	Controls m_controls = null;
    DemonBase currentDemon;
	float m_moveInputValue = 0f;
    bool m_jumped;

    public Controls Controls { get => m_controls; }

    public override void Awake()
	{
		base.Awake();

		m_controls = new Controls();
		m_controls.PlayerControls.InputMove.performed += ctx => m_moveInputValue = ctx.ReadValue<float>(); ;
		m_controls.PlayerControls.InputMove.canceled += ctx => m_moveInputValue = ctx.ReadValue<float>(); ;
        m_controls.PlayerControls.InputJump.performed += ctx => m_jumped = true;
        m_controls.PlayerControls.InputJump.canceled += ctx => m_jumped = false;
        //m_controls.PlayerControls.InputAbility.performed += ctx =>;
        //m_controls.PlayerControls.InputInteract.performed += ctx =>;
        //m_controls.PlayerControls.InputSuicide.performed += ctx =>;
        UpdateDemonReference();
	}

    // Update is called once per frame
    void Update()
    {
        if (currentDemon != null)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                PosesionManager.Instance.PossessNearestDemon(100, currentDemon);
            }
            currentDemon.Move(m_moveInputValue);
            currentDemon.ToggleWalkingParticles(m_moveInputValue != 0);
            if (m_jumped)
            {
                currentDemon.Jump();
            }
        }
        else
        {
            UpdateDemonReference();
        }
    }

    public void UpdateDemonReference()
    {
        currentDemon = PosesionManager.Instance.ControlledDemon;
    }

    /*
	void ChangeCamera()
	{
		PruebasMovement.Instance.ChangeCamera();
	}
    */
	private void OnEnable()
	{
		m_controls.Enable();
	}
	private void OnDisable()
	{
		m_controls.Disable();
	}
}
