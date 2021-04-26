using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SatanStart : MonoBehaviour
{
    [SerializeField] Satan m_satan;
    [SerializeField] AudioClip intro;
    [SerializeField] AudioClip loop;
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
        AudioManager.Instance.PlayBossMusic(intro,loop);
        m_satan.BeginFight();
    }
}
