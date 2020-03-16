using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : DemonBase
{
    [SerializeField] private float m_speed;

    public float Speed { get => m_speed; }

    public override void UseSkill()
    {
        
    }
    private void Start()
    {
        SetControlledByPlayer();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.P))
        {
            SetControlledByPlayer();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            SetNotControlledByPlayer();
        }


        if (IsControlledByPlayer)
        {
            float xInput = Input.GetAxisRaw("Horizontal");
            float zInput = Input.GetAxisRaw("Vertical");
            MyRgb.velocity = MyRgb.velocity + (Vector2.up * zInput + Vector2.right * xInput) * Speed*Time.deltaTime;
        }
        
    }
}
