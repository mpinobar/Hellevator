using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cama : MonoBehaviour
{
    [SerializeField] float m_jumpBounciness;
    [SerializeField] float m_normalBounciness;
    [SerializeField] Collider2D m_collider;
    PhysicsMaterial2D m_material;
    bool timerActive;

    [SerializeField] float timeToSetNormal = 0.25f;
    float timer;

    bool bouncyActive;
    private void Awake()
    {
        if (!m_collider)
            m_collider = GetComponent<Collider2D>();
        m_material = m_collider.sharedMaterial;
        
        timer = timeToSetNormal;
    }

    public void SetBouncy(BasicZombie character)
    {
        character.ResetJumps();
        character.MyRgb.velocity = new Vector2(character.MyRgb.velocity.x, 0);
        character.MyRgb.AddForce(Vector2.up * (character).JumpForce * m_jumpBounciness);
        //bouncyActive = false;

        //bouncyActive = true;
        //timer = timeToSetNormal;
        //Debug.LogError("Setting bouncy material for bed" + name);
        //m_material.bounciness = m_jumpBounciness;
        //timerActive = true;
    }

    //private void Update()
    //{
    //    if (bouncyActive)
    //    {
    //        timer -= Time.deltaTime;
    //        if (timer <= 0)
    //        {
    //            timer = timeToSetNormal;
    //            bouncyActive = false;
                
    //        }
    //    }
    //}
    //public void SetNormal()
    //{
    //    Debug.LogError("Setting normal material for bed" + name);
    //    m_material.bounciness = m_normalBounciness;
    //}

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.transform.TryGetComponent(out BasicZombie character))
    //    {
    //        if (bouncyActive)
    //        {
    //            if (character.IsControlledByPlayer)
    //            {
    //                character.ResetJumps();
    //                character.MyRgb.velocity = new Vector2(character.MyRgb.velocity.x, 0);
    //                character.MyRgb.AddForce(Vector2.up * (character).JumpForce * m_jumpBounciness);
    //                bouncyActive = false;
    //            }

    //        }
    //    }
    //}
}
