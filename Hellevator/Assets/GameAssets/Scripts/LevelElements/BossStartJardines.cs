using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStartJardines : MonoBehaviour
{
    [SerializeField] BossJardines m_boss;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out DemonBase demon);
        if(demon && demon.IsControlledByPlayer)
        {
            //Debug.LogError("Starting combat");
            m_boss.StartCombat();
            gameObject.SetActive(false);
        }
    }
}
