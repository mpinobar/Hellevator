using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpiderWeb : MonoBehaviour
{
    [SerializeField] float m_slowPercentage;

    Spider m_spider;

    private void Start()
    {
        m_spider = GetComponentInChildren<Spider>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (!cmpDemon.IsDead && cmpDemon.IsControlledByPlayer)
            {
                cmpDemon.CanJump = false;
                cmpDemon.Slow(m_slowPercentage);
                m_spider?.SetTarget(cmpDemon.transform);
                //Debug.LogError("Entrando a telaraña");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                cmpDemon.CanJump = true;
                cmpDemon.CancelSlow(m_slowPercentage);
                m_spider?.ReturnToInitialPosition();
                cmpDemon.ResetJumps();
                //Debug.LogError("Saliendo de telaraña");
            }
        }
    }
}
