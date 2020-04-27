using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingPit : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.transform.root.GetComponent<DemonBase>();
        if(cmpDemon != null)
        {
            //the torso has entered the pit while being a ragdoll
            if(collision == cmpDemon.LimbsColliders[0])
            {
                Destroy(cmpDemon.gameObject);
            }else if(collision == cmpDemon.PlayerCollider)
            {
                cmpDemon.Die();
            }
        }
    }
}
