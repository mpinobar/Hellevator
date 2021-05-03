using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Cinemachine;

public class SatanPhaseChangeTrigger : MonoBehaviour
{
    [SerializeField] Satan m_satan;    
    [SerializeField] Satan.Phase m_newPhase;    
    public UnityEvent OnTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
            {
                ChangePhase();
                OnTrigger?.Invoke();
            }
        }
    }

    void ChangePhase()
    {
        m_satan.BeginFight();
        m_satan.SetPhase(m_newPhase);
        gameObject.SetActive(false);        
    }
}
