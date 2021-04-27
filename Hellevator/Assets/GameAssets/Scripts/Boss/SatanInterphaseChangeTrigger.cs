using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SatanInterphaseChangeTrigger : MonoBehaviour
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
        m_satan.SetPhase(m_newPhase);
        gameObject.SetActive(false);
    }
}
