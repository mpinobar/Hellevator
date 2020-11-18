using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ButtonActivatedBase
{
    [SerializeField] Key key;
    [SerializeField] ButtonActivatedDoor m_doorToUnlock;
    [SerializeField] float m_visualLeverSpeed = 2f;
    bool m_added;
    bool m_activated;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(key.ToString()) == 1)
        {
            ActivateImmediately();
        }
    }

    public override void Activate()
    {
        base.Activate();
        if (!m_activated)
        {
            PlayerPrefs.SetInt(key.ToString(), 1);
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

    public override void ActivateImmediately()
    {
        base.ActivateImmediately();
        m_activated = true;
        transform.GetChild(0).GetChild(1).localEulerAngles = -Vector3.forward * transform.GetChild(0).GetChild(1).localEulerAngles.z;
        m_doorToUnlock.ActivateImmediately();
    }
}
