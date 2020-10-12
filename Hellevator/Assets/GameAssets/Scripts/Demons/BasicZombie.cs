using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BasicZombie : DemonBase
{
    #region Variables	
          
    
    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration = 7;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] private float m_jumpForceSecond = 10;
	[SerializeField] private bool m_canJump;
    [SerializeField] private bool m_canDoubleJump;
	[SerializeField] private float m_coyoteTimeDuration = 0f;//Mirar si hacer cambio a frames
    [SerializeField] private float m_groundCorrectionMultiplier = 3;
	private bool m_isOnLadder = false;
    private bool m_hasJumped;
    private bool m_hasDoubleJumped;
	private bool m_coyoteTimeActive = false;
	private float m_currentCoyoteTimer = 0f;
	private bool m_isHoldingJump = false;

	private bool m_jumpHasBeenPressOnAir = false;
	[SerializeField] private float m_jumpHasBeenPressOnAirTimer = 0f;
	private float m_currentTimerJumpOnAir = 0f;

	private float m_previousGravityScale = 0f;
	private Vector3 m_previousVelocity = Vector3.zero;

    [Header("References")]
    [SerializeField] ParticleSystem walkingParticles;
    [SerializeField] bool m_SoyUnNiñoDeVerdad;
	[SerializeField] GameObject skullIndicator;
	float skullOffset;
    
    [Header("Gravity")]
    [Range(1,20)]
    [Tooltip("Ascending part of the jump")]
    [SerializeField] private float m_firstGravity = 2.25f;
	[Range(1, 20)]
	[Tooltip("Ascending part of the jump when holding the jump button")]
	[SerializeField] private float m_firstGravityHoldingJump = 2.25f;
	[Range(1, 20)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_secondGravity = 2.5f;
	[Range(1, 20)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_thirdGravity = 2.5f;
    [Range(1, 20)]
    [Tooltip("Second top part of the double jump")]
    [SerializeField] private float m_thirdGravityDoubleJump = 2f;
    [Range(1, 20)]
    [Tooltip("Descending part of the jump")]
    [SerializeField] private float m_fourthGravity = 5f;
	
    #endregion

    #region Properties
    public float MaxSpeed { get => m_maxSpeed; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }
    public bool SoyUnNiñoDeVerdad { get => m_SoyUnNiñoDeVerdad; set => m_SoyUnNiñoDeVerdad = value; }

    #endregion

    public override void UseSkill()
    {
		//ShowPossessionRange();
		//PossessionManager.Instance.PossessAllDemonsInRange(MaximumPossessionRange, transform);
		//GetComponent<Petrification>().Petrify();
		GetComponent<Explosion>().CreateExplosion();
    }
    protected override void Awake()
    {
        base.Awake();
        if (skullIndicator)
        {

			skullOffset = skullIndicator.transform.position.y - m_torso.transform.position.y;
        }
        else
        {
			Debug.LogError("No se ha asgnado la referencia a la calavera que muestra el cadáver a poseer. Nombre de cuerpo: " + name);
        }
    }

    protected override void Update()
    {

		
        base.Update();

        

		if (CanMove)
		{
			
			if (!IsGrounded())
			{
				//in the air while jumping
				if (!m_hasJumped && !m_coyoteTimeActive)
				{
					m_coyoteTimeActive = true;
					m_currentCoyoteTimer = m_coyoteTimeDuration;
				}
				else if (m_coyoteTimeActive)
				{
					m_currentCoyoteTimer -= Time.deltaTime;
					if (m_currentCoyoteTimer <= 0)
					{
						m_hasJumped = true;
						m_coyoteTimeActive = false;
					}
				}

				if (m_jumpHasBeenPressOnAir)
				{
					m_currentTimerJumpOnAir = m_currentTimerJumpOnAir - Time.deltaTime;
					if (m_currentTimerJumpOnAir <= 0)
					{
						m_jumpHasBeenPressOnAir = false;
					}
				}


				//ascending part of the jump
				if (MyRgb.velocity.y > 1)
				{
					if (m_isHoldingJump)
					{
						MyRgb.gravityScale = m_firstGravityHoldingJump;
					}
					else
					{
						MyRgb.gravityScale = m_firstGravity;
					}

				}
				else if (MyRgb.velocity.y > 0)
				{
					MyRgb.gravityScale = m_secondGravity;
				}
				else if (MyRgb.velocity.y > -1)
				{
					if (m_hasDoubleJumped)
					{
						MyRgb.gravityScale = m_thirdGravityDoubleJump;
					}
					else
					{
						MyRgb.gravityScale = m_thirdGravity;
					}
				}
				else
				{
					MyRgb.gravityScale = m_fourthGravity;
				}

				ToggleWalkingParticles(false);
			}
			else
			{
				MyRgb.gravityScale = 2;
				m_coyoteTimeActive = false;

			}
		}
		m_myAnimator.SetFloat("xMovement", Mathf.Abs(MyRgb.velocity.x * 0.1f));
		VerticalMovementOnLadder(InputManager.Instance.VerticalInputValue);
		if (skullIndicator)
        {
			if (IsDead && PossessionManager.Instance.ControlledDemon != null && !IsInDanger)
			{
				DistanceToPlayer = Vector2.Distance(transform.position, PossessionManager.Instance.ControlledDemon.transform.position);
				if (DistanceToPlayer <= PossessionManager.Instance.ControlledDemon.MaximumPossessionRange)
				{
					if (PossessionManager.Instance.DemonShowingSkull == null || PossessionManager.Instance.DemonShowingSkull == this || (PossessionManager.Instance.DemonShowingSkull != this && PossessionManager.Instance.DemonShowingSkull.DistanceToPlayer > DistanceToPlayer))
					{
						PossessionManager.Instance.DemonShowingSkull = this;
						skullIndicator.SetActive(true);
						skullIndicator.transform.position = m_torso.transform.position + Vector3.up * skullOffset;
					}
					else
					{
						skullIndicator.SetActive(false);
					}

				}
				else
				{

					skullIndicator.SetActive(false);
				}
			}
			else
			{
				skullIndicator.SetActive(false);
			}
		}
		
	}

	public void VerticalMovementOnLadder(float verticalInput)
    {
        if (m_isOnLadder)
        {
			if(!m_isJumping || (m_isJumping && MyRgb.velocity.y < 0))
            {
				MyRgb.gravityScale = 0f;
				MyRgb.velocity = new Vector2(MyRgb.velocity.x, verticalInput * MaxSpeed);
			}
			
        }
    }

    public override void Move(float xInput)
    {		       
        float accel = m_acceleration;
       
        if (IsGrounded())
        {
            if ((MyRgb.velocity.x) * xInput < 0)
            {
                accel *= m_groundCorrectionMultiplier;
            }
        }
        
        MyRgb.velocity = new Vector2(Mathf.MoveTowards(MyRgb.velocity.x, xInput * MaxSpeed, accel * Time.deltaTime), MyRgb.velocity.y);
    }

    public override void Jump()
    {
        if (m_canJump)
        {
			if (m_jumpHasBeenPressOnAir)
			{
				m_jumpHasBeenPressOnAir = false;
			}
            if (!m_hasJumped)
            {
				m_isJumping = true;
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * JumpForce);
                m_hasJumped = true;
				m_coyoteTimeActive = false;
				m_isHoldingJump = true;
                m_myAnimator.SetTrigger("Jump");				
                MusicManager.Instance.PlayAudioSFX(m_jumpClip,false);
            }
            else if(m_canDoubleJump && !m_hasDoubleJumped)
            {
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * m_jumpForceSecond);
                m_hasDoubleJumped = true;
                m_myAnimator.SetTrigger("Jump");
                MusicManager.Instance.PlayAudioSFX(m_jumpClip, false);
            }
			else if(m_hasJumped)
			{
				if (m_canDoubleJump && m_hasDoubleJumped)
				{
					m_currentTimerJumpOnAir = m_jumpHasBeenPressOnAirTimer;
					m_isHoldingJump = true;
					m_jumpHasBeenPressOnAir = true;
				}
				else
				{
					m_currentTimerJumpOnAir = m_jumpHasBeenPressOnAirTimer;
					m_isHoldingJump = true;
					m_jumpHasBeenPressOnAir = true;
				}
			}
        }
    }

	public override void JumpReleaseButton()
	{
		m_isHoldingJump = false;
	}

	public override void ToggleWalkingParticles(bool active)
    {
        //walkingParticles.Stop();
        //if (active)
        //{
        //    walkingParticles.Play();
        //}
        //else
        //{
            
        //}
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            if (m_canJump)
            {
                if (m_hasJumped)
                {
                    MusicManager.Instance.PlayAudioSFX(m_landingClip, false);
					m_isJumping = false;
                }
                m_hasJumped = false;
                
                if (m_canDoubleJump)
                {
                    m_hasDoubleJumped = false;
					
					if (m_jumpHasBeenPressOnAir && m_isHoldingJump)
					{
						Jump();
						m_jumpHasBeenPressOnAir = false;
					}																														
				}
				if (m_jumpHasBeenPressOnAir && m_isHoldingJump)
				{
					Jump();
					m_jumpHasBeenPressOnAir = false;
				}
			}
        }
    }

	public override void StopMovement()
	{
		MyRgb.velocity = Vector3.zero;
		m_previousGravityScale = MyRgb.gravityScale;
		MyRgb.gravityScale = 0;
	}

	public override void ContinueMovement()
	{
		MyRgb.gravityScale = m_previousGravityScale;
	}


	public void SetOnLadder(bool onLadder)
    {
		m_isOnLadder = onLadder;
    }
    
}
