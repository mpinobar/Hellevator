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
    private bool m_hasJumped;
    private bool m_hasDoubleJumped;
	private bool m_coyoteTimeActive = false;
	private float m_currentCoyoteTimer = 0f;

    [Header("References")]
    [SerializeField] ParticleSystem walkingParticles;
    
    
    [Header("Gravity")]
    [Range(1,10)]
    [Tooltip("Ascending part of the jump")]
    [SerializeField] private float m_firstGravity = 2.25f;
    [Range(1, 10)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_secondGravity = 2.5f;
	[Range(1, 10)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_thirdGravity = 2.5f;
    [Range(1, 10)]
    [Tooltip("Second top part of the jump")]
    [SerializeField] private float m_thirdGravityDoubleJump = 2f;
    [Range(1, 10)]
    [Tooltip("Descending part of the jump")]
    [SerializeField] private float m_fourthGravity = 5f;
	
    #endregion

    #region Properties
    public float MaxSpeed { get => m_maxSpeed; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }

    #endregion
	
    public override void UseSkill()
    {
        
    }
        

    protected override void Update()
    {
        base.Update();

        //in the air while jumping
        if (!IsGrounded())
        {
			if (!m_hasJumped && !m_coyoteTimeActive)
			{
				m_coyoteTimeActive = true;
				m_currentCoyoteTimer = m_coyoteTimeDuration;
			}
			else if (m_coyoteTimeActive)
			{
				m_currentCoyoteTimer = m_currentCoyoteTimer - Time.deltaTime;
				if(m_currentCoyoteTimer <= 0)
				{
					m_hasJumped = true;
					m_coyoteTimeActive = false;
				}
			}
			
            //ascending part of the jump
            if (MyRgb.velocity.y > 1)
            {
                MyRgb.gravityScale = m_firstGravity;
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
        m_myAnimator.SetBool("Walking", Mathf.Abs(MyRgb.velocity.x) > 0.2f);
    }

    

    public override void Move(float xInput)
    {
        MyRgb.velocity = new Vector2(Mathf.MoveTowards(MyRgb.velocity.x, xInput * MaxSpeed, Acceleration * Time.deltaTime),MyRgb.velocity.y);
    }

    public override void Jump()
    {
        if (m_canJump)
        {
            if (!m_hasJumped)
            {
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * JumpForce);
                m_hasJumped = true;
				m_coyoteTimeActive = false;
            }
            else if(m_canDoubleJump && !m_hasDoubleJumped)
            {
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * m_jumpForceSecond);
                m_hasDoubleJumped = true;
            }
        }
    }
    
    public override void ToggleWalkingParticles(bool active)
    {
        if (active)
        {
            walkingParticles.Play();
        }
        else
        {
            walkingParticles.Stop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            if (m_canJump)
            {
                m_hasJumped = false;
                if (m_canDoubleJump)
                {
                    m_hasDoubleJumped = false;
                }
            }
        }
    }

}
