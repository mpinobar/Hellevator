using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : DemonBase
{
    #region Variables

    [Header("Movement")]
    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration = 7;
    [SerializeField] private float m_jumpForce = 10;

    [Header("References")]
    [SerializeField] ParticleSystem walkingParticles;
    [SerializeField] LayerMask m_JumpMask;

    private bool m_isJumping;

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

        print(IsGrounded());
        //in the air while jumping
        if (!IsGrounded())
        {
            //ascending part of the jump
            if (MyRgb.velocity.y > 1)
            {
                MyRgb.gravityScale = 2.25f;
            }
            else if (MyRgb.velocity.y > 0)
            {
                MyRgb.gravityScale = 2.5f;
            }
            else if (MyRgb.velocity.y > -1)
            {
                MyRgb.gravityScale = 2;
            }
            else
            {
                MyRgb.gravityScale = 5;
            }

        }
        else
        {
            MyRgb.gravityScale = 2;
        }
    }


    public override void Move(float xInput)
    {
        MyRgb.velocity = new Vector2(Mathf.MoveTowards(MyRgb.velocity.x, xInput * MaxSpeed, Acceleration * Time.deltaTime),MyRgb.velocity.y);
           // Vector2.MoveTowards(MyRgb.velocity, Vector2.right * xInput * MaxSpeed + MyRgb.velocity.y * Vector2.up, Acceleration * Time.deltaTime);
    }

    public override void Jump()
    {
        if (IsGrounded() && !m_isJumping)
        {
            MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
            MyRgb.AddForce(Vector2.up * JumpForce);
            m_isJumping = true;
        }
    }


    private bool IsGrounded()
    {
        RaycastHit2D[] impact = Physics2D.CircleCastAll(transform.position, 0.5f, Vector2.down, 2,m_JumpMask);
        bool isGrounded = false;
        for (int i = 0; i < impact.Length; i++)
        {
            if (impact[i].transform.root != transform)
            {
                isGrounded = true;
                m_isJumping = false;
            }
        }
        return isGrounded;
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
}
