using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ButtonActivatedBase
{
    [SerializeField] ButtonActivatedDoor m_doorToUnlock;
    [SerializeField] float m_visualLeverSpeed = 2f;
    bool m_added;
    bool m_activated;
    public override void Activate()
    {
        if (!m_activated)
        {
            m_doorToUnlock.Activate();
            ChangeLeverVisual();
            m_activated = true;
        }
        
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
        
        StartCoroutine(LeverVisualRotation());
    }

    IEnumerator LeverVisualRotation()
    {
        Transform visual = transform.GetChild(0).GetChild(1);
        float currentAngle = visual.localEulerAngles.z;
        float endAngle = -currentAngle;

        while (currentAngle > endAngle)
        {
            
            currentAngle -= Time.deltaTime * m_visualLeverSpeed;
            visual.localEulerAngles = Vector3.forward * currentAngle;
            yield return null;
        }
    }
}
