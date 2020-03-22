using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : DemonBase
{
    #region Variables

    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration = 7;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] ParticleSystem walkingParticles;

    private bool  m_isGrounded;

    #endregion

    #region Properties
    public float MaxSpeed { get => m_maxSpeed; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }
    #endregion


    public override void UseSkill()
    {
        
    }




    public override void Move(float xInput)
    {
        MyRgb.velocity = Vector2.MoveTowards(MyRgb.velocity, Vector2.right * xInput * MaxSpeed, Acceleration * Time.deltaTime);
    }

    public override void Jump()
    {
        if (m_isGrounded)
        {
            MyRgb.AddForce(Vector2.up * JumpForce);
            m_isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //collision detection for jump reset
        RaycastHit2D [] impact = Physics2D.CircleCastAll(transform.position,0.5f, Vector2.down,2);        
        for (int i = 0; i < impact.Length; i++)
        {
            if(collision.collider == impact[i].collider)
            {
                m_isGrounded = true;
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
}
