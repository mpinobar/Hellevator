using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLogicalCollider : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if (collision.gameObject.layer == 0 << 0)
        {
            transform.parent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            transform.parent.GetComponent<Rigidbody2D>().angularVelocity = 0;
            //for (int i = 0; i < LimbsColliders.Length; i++)
            //{
            //    LimbsColliders[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //}
        }

    }
}
