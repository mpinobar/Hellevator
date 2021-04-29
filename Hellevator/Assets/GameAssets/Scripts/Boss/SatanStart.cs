using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatanStart : MonoBehaviour
{
    [SerializeField] Satan m_satan;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
                StartBossFight();
            
        }
    }

    public void StartBossFight()
    {        
        m_satan.BeginFight();
    }
}
