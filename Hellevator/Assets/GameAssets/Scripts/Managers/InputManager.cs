using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : PersistentSingleton<InputManager>
{

    Controls    m_controls = null;
    DemonBase   m_currentDemon;
    float       m_moveInputValue = 0f;
    bool        m_jumped;
    bool        m_isInInteactionTrigger = false;
    float       m_verticalInputValue;
    Vector3     m_direction = Vector3.one;
     
    public delegate void OnButtonPress();

    public event OnButtonPress OnInteract;

    List<DemonBase> m_extraDemonsControlled;

    public Controls Controls
    {
        get => m_controls;
    }
    public bool IsInInteactionTrigger
    {
        get => m_isInInteactionTrigger; set => m_isInInteactionTrigger = value;
    }
    public float VerticalInputValue { get => m_verticalInputValue; set => m_verticalInputValue = value; }

    public override void Awake()
    {
        base.Awake();

        m_controls = new Controls();
        m_controls.PlayerControls.InputMove.performed += ctx => m_moveInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputMove.canceled += ctx => m_moveInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputJump.performed += ctx => Jump();
        m_controls.PlayerControls.InputJump.canceled += ctx => JumpButtonReleased();
        m_controls.PlayerControls.VerticalMovement.performed += ctx => m_verticalInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.VerticalMovement.canceled += ctx => m_verticalInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputInteract.performed += ctx => Interact();
        m_controls.PlayerControls.InputAbility.performed += ctx => UseSkill();
        //m_controls.PlayerControls.InputInteract.performed += ctx => Grab();
        //m_controls.PlayerControls.InputSuicide.performed += ctx => PossesNearestDemon();
        UpdateDemonReference();
    }


    public void ResetPlayerInput()
    {
        m_moveInputValue = 0;
        m_verticalInputValue = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        FeedInputToMainDemon();

        if (PossessionManager.Instance.ControllingMultipleDemons)
        {
            FeedInputToExtraDemons();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            LevelManager.Instance.StartRestartingLevel();
        }
    }

    private void LateUpdate()
    {
        SetMainCharacterDirection();
        SetExtraCharactersDirections();
    }

    private void FeedInputToExtraDemons()
    {
        for (int i = 0; i < m_extraDemonsControlled.Count; i++)
        {
            if (m_extraDemonsControlled[i].CanMove)
            {
                m_extraDemonsControlled[i].Move(m_moveInputValue);

                m_extraDemonsControlled[i].ToggleWalkingParticles(m_moveInputValue != 0 && m_extraDemonsControlled[i].IsGrounded());

                if (m_moveInputValue > 0)
                {
                    if (((BasicZombie)m_extraDemonsControlled[i]).SoyUnNiñoDeVerdad)
                    {
                        m_extraDemonsControlled[i].MovementDirection = 1;
                    }
                    else
                    {
                        m_extraDemonsControlled[i].MovementDirection = -1;
                    }

                }
                else if (m_moveInputValue < 0)
                {

                    if (((BasicZombie)m_extraDemonsControlled[i]).SoyUnNiñoDeVerdad)
                    {
                        m_extraDemonsControlled[i].MovementDirection = -1;
                    }
                    else
                    {
                        m_extraDemonsControlled[i].MovementDirection = 1;
                    }

                }
                if (m_extraDemonsControlled[i].MovementDirection != 0)
                    m_extraDemonsControlled[i].transform.localScale = Vector3.one - (Vector3.right * (1 - m_extraDemonsControlled[i].MovementDirection));
            }
            else if (!m_extraDemonsControlled[i].CanMove)
            {
                m_extraDemonsControlled[i].Move(0);
            }
        }
    }

    private void FeedInputToMainDemon()
    {
        if (m_currentDemon != null && m_currentDemon.CanMove)
        {
            m_currentDemon.Move(m_moveInputValue);

            m_currentDemon.ToggleWalkingParticles(m_moveInputValue != 0 && m_currentDemon.IsGrounded());

            if (m_moveInputValue > 0)
            {
                if (((BasicZombie)m_currentDemon).SoyUnNiñoDeVerdad)
                {
                    m_currentDemon.MovementDirection = 1;
                }
                else
                {
                    m_currentDemon.MovementDirection = -1;
                }

            }
            else if (m_moveInputValue < 0)
            {

                if (((BasicZombie)m_currentDemon).SoyUnNiñoDeVerdad)
                {
                    m_currentDemon.MovementDirection = -1;
                }
                else
                {
                    m_currentDemon.MovementDirection = 1;
                }

            }
            
            //m_currentDemon.transform.localScale = direction; //Vector3.one - (Vector3.right * (1 - m_currentDemon.MovementDirection));
        }
        else if (m_currentDemon != null && !m_currentDemon.CanMove)
        {
            m_currentDemon.Move(0);
        }
        else
        {
            UpdateDemonReference();
        }
    }

    public void SetExtraCharactersDirections()
    {
        if(m_extraDemonsControlled != null && m_extraDemonsControlled.Count > 0)
        {
            for (int i = 0; i < m_extraDemonsControlled.Count; i++)
            {
                m_extraDemonsControlled[i].transform.localScale = m_direction;
            }
        }
    }

    private void SetMainCharacterDirection()
    {
        if (m_currentDemon != null && m_currentDemon.MovementDirection != 0)
        {
            m_direction = transform.localScale;
            m_direction.x = m_currentDemon.MovementDirection;
            m_currentDemon.transform.localScale = m_direction;
        }
    }
    void Interact()
    {
        if (IsInInteactionTrigger)
        {
            OnInteract();
        }
    }

    void Jump()
    {
        if (m_currentDemon != null && m_currentDemon.CanMove && !m_isInInteactionTrigger)
        {
            m_currentDemon.ToggleWalkingParticles(false);
            m_currentDemon.Jump();

        }
        if (m_isInInteactionTrigger)
        {
            OnInteract();
        }
        if (PossessionManager.Instance.ControllingMultipleDemons)
        {
            for (int i = 0; i < m_extraDemonsControlled.Count; i++)
            {
                if (m_extraDemonsControlled[i].CanMove)
                {
                    m_extraDemonsControlled[i].Jump();
                }
            }
        }



    }
    void JumpButtonReleased()
    {
        if (m_currentDemon != null && m_currentDemon.CanMove)
            m_currentDemon.JumpReleaseButton();

        if (PossessionManager.Instance.ControllingMultipleDemons)
        {

            for (int i = 0; i < m_extraDemonsControlled.Count; i++)
            {
                if (m_extraDemonsControlled[i].CanMove)
                {
                    m_extraDemonsControlled[i].JumpReleaseButton();
                }
            }
        }

    }



    void PossesNearestDemon()
    {
        if (m_currentDemon != null && m_currentDemon.CanMove)
            m_currentDemon.Die(true);

        if (PossessionManager.Instance.ControllingMultipleDemons)
        {
            for (int i = 0; i < m_extraDemonsControlled.Count; i++)
            {
                if (m_extraDemonsControlled[i].CanMove)
                {
                    m_extraDemonsControlled[i].Die(true);
                }
            }
        }

    }

    void UseSkill()
    {
        //if (m_currentDemon != null && m_currentDemon.CanMove)
        //    m_currentDemon.UseSkill();
        if (m_currentDemon != null && m_currentDemon.CanMove)
            m_currentDemon.ShowPossessionRange();

        //if (PossessionManager.Instance.ControllingMultipleDemons)
        //{
        //    for (int i = 0; i < extraDemonsControlled.Count; i++)
        //    {
        //        if (extraDemonsControlled[i].CanMove)
        //        {
        //            extraDemonsControlled[i].UseSkill();
        //        }
        //    }
        //}

    }

    public void UpdateDemonReference()
    {
        m_currentDemon = PossessionManager.Instance.ControlledDemon;
    }

    public void UpdateExtraDemonsControlled(List<DemonBase> controlledDemons)
    {
        if (m_extraDemonsControlled == null)
        {
            m_extraDemonsControlled = new List<DemonBase>();
        }

        for (int i = 0; i < controlledDemons.Count; i++)
        {
            m_extraDemonsControlled.Add(controlledDemons[i]);
        }
    }

    public void RemoveExtraDemonControlled(DemonBase demonToRemove)
    {
        if (m_extraDemonsControlled.Contains(demonToRemove))
        {
            m_extraDemonsControlled.Remove(demonToRemove);
        }
        else
        {
            Debug.LogError("Trying to remove a demon that wasn't possessed. Demon is " + demonToRemove.name);
        }
    }

    /*
	public void Grab()
	{
		m_currentDemon.Grab();
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
