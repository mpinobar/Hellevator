using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ButtonActivatedBase
{
    [SerializeField] ButtonActivatedDoor m_doorToUnlock;
    bool m_added;

    public override void Activate()
    {
        m_doorToUnlock.Activate();
        ChangeLeverVisual();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_added)
        {
            if (collision.GetComponentInParent<DemonBase>())
            {
                if (collision.GetComponentInParent<DemonBase>().IsControlledByPlayer)
                {
                    InputManager.Instance.IsInInteactionTrigger = true;
                    InputManager.Instance.OnInteract += Activate;
                    m_added = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_added)
        {
            if (collision.GetComponentInParent<DemonBase>())
            {
                if (collision.GetComponentInParent<DemonBase>().IsControlledByPlayer)
                {
                    InputManager.Instance.IsInInteactionTrigger = false;
                    InputManager.Instance.OnInteract -= Activate;
                    m_added = false;
                }
            }
        }        
    }

    void ChangeLeverVisual()
    {
        transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
    }
}
