using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : DemonBase
{
    [SerializeField] private float m_speed;
    [SerializeField] private float m_acceleration = 7;
    [SerializeField] private float m_jumpForce = 10;
    public float Speed { get => m_speed; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }


    public bool possessing;
    public override void UseSkill()
    {
        
    }
    private void Start()
    {
        if(possessing)
        SetControlledByPlayer();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
        {
            PossessNearestDemon(10);
        }
        


        if (IsControlledByPlayer)
        {
            float xInput = Input.GetAxisRaw("Horizontal");            
            MyRgb.velocity = Vector2.MoveTowards(MyRgb.velocity, Vector2.right * xInput * Speed, Acceleration * Time.deltaTime);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MyRgb.AddForce(Vector2.up*JumpForce);
            }
        }
    }
}
