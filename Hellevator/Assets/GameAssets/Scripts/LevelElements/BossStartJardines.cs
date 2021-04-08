using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStartJardines : MonoBehaviour
{
    [SerializeField] BossJardines m_boss;
    [SerializeField] GameObject m_bossWall;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out DemonBase demon);
        if(demon && demon.IsControlledByPlayer)
        {
            //Debug.LogError("Starting combat");
            demon.MaximumPossessionRange *= 100;
            m_boss.StartCombat();
            gameObject.SetActive(false);
            m_bossWall.SetActive(true);
        }
    }
}
