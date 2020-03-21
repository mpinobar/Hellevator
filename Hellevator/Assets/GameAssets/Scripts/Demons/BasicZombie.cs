using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : DemonBase
{
    #region Variables

    [SerializeField] private float m_maxSpeed;
    [SerializeField] private float m_acceleration = 7;
    [SerializeField] private float m_jumpForce = 10;
    [SerializeField] private bool  m_possessedOnStart;

    private bool  m_canJump;

    #endregion

    #region Properties
    public float MaxSpeed { get => m_maxSpeed; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }
    #endregion


    public override void UseSkill()
    {
        
    }
    private void Start()
    {
        if (m_possessedOnStart)
        {
            SetControlledByPlayer();
        }
        else
        {
            SetNotControlledByPlayer();
        }
        m_canJump = true;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P) && IsControlledByPlayer)
        {
            PosesionManager.Instance.PossessNearestDemon(100,this);
        }
        
        if (IsControlledByPlayer)
        {
            //horizontal movement
            float xInput = Input.GetAxisRaw("Horizontal");            
            MyRgb.velocity = Vector2.MoveTowards(MyRgb.velocity, Vector2.right * xInput * MaxSpeed, Acceleration * Time.deltaTime);

            //jumping
            if (Input.GetKeyDown(KeyCode.Space) && m_canJump)
            {
                MyRgb.AddForce(Vector2.up*JumpForce);
                m_canJump = false;
            }
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
                m_canJump = true;
            }
        }
    }
}
