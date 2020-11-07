using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLogicalCollider : MonoBehaviour
{

    DemonBase m_parentDemon;

    private void Start()
    {
        m_parentDemon = GetComponentInParent<DemonBase>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.layer == 0 << 0 || collision.gameObject.layer == 1 << 8)
        {
            if (m_parentDemon)
            {   
                //m_parentDemon.Torso
                //Debug.LogError("Resetting ragdoll velocity of " + m_parentDemon.name);
                m_parentDemon.ResetRagdollVelocity();
            }
            //for (int i = 0; i < LimbsColliders.Length; i++)
            //{
            //    LimbsColliders[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //}
        }

    }
}
