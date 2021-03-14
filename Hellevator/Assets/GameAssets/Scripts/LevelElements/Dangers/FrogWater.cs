using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogWater : MonoBehaviour
{
    [SerializeField] AmphibianDemon m_frog;
     
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                m_frog?.StartChase(cmpDemon.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                m_frog?.ReturnToPatrol();
            }
        }
    }
}
