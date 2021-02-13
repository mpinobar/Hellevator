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
    bool        m_isInMenu;
    bool        m_canControlCharacters = true;
    bool        m_throwingHead = false;
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
    public bool IsInMenu { get => m_isInMenu; set => m_isInMenu = value; }
    public bool ThrowingHead { get => m_throwingHead; set => m_throwingHead = value; }

    public override void Awake()
    {
        base.Awake();

        m_controls = new Controls();
        m_controls.PlayerControls.InputMove.performed += ctx => m_moveInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputMove.canceled += ctx => m_moveInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputJump.performed += ctx => Jump();
        m_controls.PlayerControls.InputJump.canceled += ctx => JumpButtonReleased();
        m_controls.PlayerControls.VerticalMovement.performed += ctx => VerticalInputStart(ctx.ReadValue<float>());
        m_controls.PlayerControls.VerticalMovement.performed += ctx => m_verticalInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.VerticalMovement.canceled += ctx => m_verticalInputValue = ctx.ReadValue<float>();
        m_controls.PlayerControls.InputInteract.performed += ctx => Interact();
        m_controls.PlayerControls.InputShowRange.performed += ctx => UseSkill();
        m_controls.PlayerControls.InputPosMulti.performed += ctx => ToggleMultiplePosseion();
        m_controls.PlayerControls.InputMenu.performed += ctx => InputMenu();
        //m_controls.PlayerControls.InputSuicide.performed += ctx => PossesNearestDemon();
        UpdateDemonReference();
    }

    public void InputMenu()
    {
        if (!IsInMenu)
        {
            UIController.Instance.ShowPauseMenu();
        }
        else
        {
            UIController.Instance.Resume();
        }
    }

    public void ResetPlayerInput()
    {
        m_moveInputValue = 0;
        m_verticalInputValue = 0;
    }
    public void ResetPlayerHorizontalInput()
    {
        m_moveInputValue = 0;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!m_isInMenu)
        {
            if (m_canControlCharacters)
            {
                FeedInputToMainDemon();

                if (PossessionManager.Instance.ControllingMultipleDemons)
                {
                    FeedInputToExtraDemons();
                }

                if (Input.GetKeyDown(KeyCode.R) && !FadeManager.IsRestarting)
                {
                    
                    LevelManager.Instance.StartRestartingLevelNoDelay();
                }
            }
            
        }
        else
        {
            FeedInputToMenuNavigation();
        }
        if (Input.GetKeyDown(KeyCode.J))
        {
            UIController.Instance.UnlockMap();
        }
    }

    void FeedInputToMenuNavigation()
    {
        UIController.Instance.NavigateMenu(m_moveInputValue, m_verticalInputValue);
    }

    private void LateUpdate()
    {
        if(m_canControlCharacters && !m_isInMenu)
        {
            SetMainCharacterDirection();
            SetExtraCharactersDirections();
        }       
    }

    private void FeedInputToExtraDemons()
    {
        for (int i = 0; i < m_extraDemonsControlled.Count; i++)
        {
            if(m_extraDemonsControlled[i] != null)
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
            else
            {
                m_extraDemonsControlled.RemoveAt(i);
                i--;
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
        if (m_extraDemonsControlled != null && m_extraDemonsControlled.Count > 0)
        {
            for (int i = 0; i < m_extraDemonsControlled.Count; i++)
            {
                if(m_extraDemonsControlled[i].MovementDirection == 0)
                {
                    m_extraDemonsControlled[i].transform.localScale = new Vector3(Mathf.Abs(m_extraDemonsControlled[i].transform.localScale.x) / m_extraDemonsControlled[i].transform.lossyScale.x, m_extraDemonsControlled[i].transform.localScale.y / m_extraDemonsControlled[i].transform.lossyScale.y, 1);
                }
                else
                    m_extraDemonsControlled[i].transform.localScale = new Vector3(Mathf.Abs(m_extraDemonsControlled[i].transform.localScale.x) * m_extraDemonsControlled[i].MovementDirection / m_extraDemonsControlled[i].transform.lossyScale.x, m_extraDemonsControlled[i].transform.localScale.y / m_extraDemonsControlled[i].transform.lossyScale.y, 1);
            }
        }
    }

    private void SetMainCharacterDirection()
    {
        if (m_currentDemon != null)
        {
            if (m_currentDemon.MovementDirection == 0)
            {
                m_currentDemon.transform.localScale = new Vector3(Mathf.Abs(m_currentDemon.transform.localScale.x) / m_currentDemon.transform.lossyScale.x, m_currentDemon.transform.localScale.y / m_currentDemon.transform.lossyScale.y, 1);
            }
            else
                m_currentDemon.transform.localScale = new Vector3(Mathf.Abs(m_currentDemon.transform.localScale.x) * m_currentDemon.MovementDirection / m_currentDemon.transform.lossyScale.x, m_currentDemon.transform.localScale.y / m_currentDemon.transform.lossyScale.y, 1);
            //m_direction = PossessionManager.Instance.ControlledDemon.MovementDirection;
            //Debug.LogError(m_currentDemon.transform.localScale);
        }

    }
    void Interact()
    {
        if (IsInInteactionTrigger)
        {
            OnInteract();
        }
    }

	void ToggleMultiplePosseion()
	{
		//PossessionManager.Instance.ToggleMultiplePossesion();
	}

    void Jump()
    {
        if (!IsInMenu)
        {
            if (PossessionManager.Instance.ChoosingWhenDead)
            {
                PossessionManager.Instance.ChoosingWhenDead = false;
                PossessionManager.Instance.MultiplePossessionWhenDead = true;
                UIController.Instance.EndDecisionTime();
            }else if (ThrowingHead)
            {
                PossessionManager.Instance.ControlledDemon.GetComponent<Catapult>().ThrowHead();
            }
            else
            {
                if (m_currentDemon != null && m_currentDemon.CanMove /*&& !m_isInInteactionTrigger*/)
                {
                    m_currentDemon.ToggleWalkingParticles(false);
                    m_currentDemon.Jump();

                }
                //if (m_isInInteactionTrigger)
                //{
                //    OnInteract();
                //}
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

           
        }
        else
        {
            UIController.Instance.Selected.Press();
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

    public void VerticalInputStart(float verticalInput)
    {
        if (verticalInput < 0)
        {
            if (m_currentDemon != null && m_currentDemon.CanMove)
                ((BasicZombie)m_currentDemon).CheckTraversePlatform();

            if (PossessionManager.Instance.ControllingMultipleDemons)
            {
                for (int i = 0; i < m_extraDemonsControlled.Count; i++)
                {
                    if (m_extraDemonsControlled[i].CanMove)
                    {
                        ((BasicZombie)m_extraDemonsControlled[i]).CheckTraversePlatform();
                    }
                }
            }
        }
    }

    /// <summary>
    /// Player input wont be fed into possessed characters
    /// </summary>
    public void ReleasePlayerInput()
    {
        m_canControlCharacters = false;
        //ResetPlayerInput();
        //m_currentDemon.Move(0);
    }

    /// <summary>
    /// Player input will be fed into possessed characters
    /// </summary>
    public void RegainPlayerInput()
    {
        m_canControlCharacters = true;
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

    public void RemoveAllExtraDemonsControlled()
    {
        if(m_extraDemonsControlled != null)
            m_extraDemonsControlled.Clear();
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
