using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{

	Controls m_controls = null;
    DemonBase m_currentDemon;
	float m_moveInputValue = 0f;
    bool m_jumped;

    public Controls Controls { get => m_controls; }

    public override void Awake()
	{
		base.Awake();

		m_controls = new Controls();
        m_controls.PlayerControls.InputMove.performed += ctx => m_moveInputValue = ctx.ReadValue<float>(); 
		m_controls.PlayerControls.InputMove.canceled += ctx => m_moveInputValue = ctx.ReadValue<float>(); 
        m_controls.PlayerControls.InputJump.performed += ctx => Jump();
        m_controls.PlayerControls.InputAbility.performed += ctx => UseSkill();
        //m_controls.PlayerControls.InputInteract.performed += ctx =>;
        m_controls.PlayerControls.InputSuicide.performed += ctx => PossesNearestDemon();
        UpdateDemonReference();
	}



    // Update is called once per frame
    protected virtual void Update()
    {
        if (m_currentDemon != null)
        {            
            m_currentDemon.Move(m_moveInputValue);
            
            m_currentDemon.ToggleWalkingParticles(m_moveInputValue != 0 && m_currentDemon.IsGrounded());

            if (m_moveInputValue > 0)
            {
                m_currentDemon.MovementDirection = 1;
                m_currentDemon.MySprite.flipX = false;
            }
            else if (m_moveInputValue < 0)
            {
                m_currentDemon.MovementDirection = -1;
                m_currentDemon.MySprite.flipX = true;
            }
        }
        else
        {
            UpdateDemonReference();
        }
    }

	void Jump()
	{
        m_currentDemon.ToggleWalkingParticles(false);
		m_currentDemon.Jump();
	}

	void PossesNearestDemon()
	{
        m_currentDemon.Die();
    }

	void UseSkill()
	{
		m_currentDemon.UseSkill();
	}

    public void UpdateDemonReference()
    {
        m_currentDemon = PosesionManager.Instance.ControlledDemon;
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
