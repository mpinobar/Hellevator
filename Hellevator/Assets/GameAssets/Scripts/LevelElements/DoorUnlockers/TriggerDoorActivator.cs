using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorActivator : MonoBehaviour
{
    [SerializeField] ActivatedBase m_doorToActivate;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
                m_doorToActivate.Activate();
        }
    }
}
