using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateSpawner : MonoBehaviour
{
    [SerializeField] Spawner m_spawnerToDeactivate;
       
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase demon = collision.GetComponentInParent<DemonBase>();
        if (demon != null && demon.IsControlledByPlayer)
        {
            m_spawnerToDeactivate.enabled = false;
        }

    }
}
