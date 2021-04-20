using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SatanInterphaseChangeTrigger : MonoBehaviour
{
    [SerializeField] Satan m_satan;    
    [SerializeField] Satan.Phase m_newPhase;    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
                ChangePhase();
        }
    }

    void ChangePhase()
    {
        m_satan.SetPhase(m_newPhase);
        gameObject.SetActive(false);
    }
}
