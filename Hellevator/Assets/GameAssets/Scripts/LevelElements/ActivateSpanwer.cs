using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSpanwer : MonoBehaviour
{
    [SerializeField] Spawner m_spawnerToActivate;


    private void Start()
    {
        m_spawnerToActivate.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
		print(collision.transform.root.name);
        DemonBase demon = collision.transform.root.GetComponent<DemonBase>();
		print(demon.name);
        if(demon != null && demon.IsControlledByPlayer)
        {
            m_spawnerToActivate.enabled = true;
        }
        
    }
}
