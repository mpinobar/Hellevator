using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogWater : MonoBehaviour
{
    [SerializeField] AmphibianDemon m_frog;
    bool m_active;

    public bool Active { get => m_active; set => m_active = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Active && collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                m_frog?.StartChase(cmpDemon.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (Active && collision.TryGetComponent(out BasicZombie cmpDemon))
        //{
        //    if (cmpDemon.IsControlledByPlayer)
        //    {
        //        m_frog?.ReturnToPatrol();
        //    }
        //}
    }
}
