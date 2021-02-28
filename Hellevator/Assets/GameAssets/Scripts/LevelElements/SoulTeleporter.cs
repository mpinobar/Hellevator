using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulTeleporter : MonoBehaviour
{
    [SerializeField] float m_rotationSpeed = 540f;
    [SerializeField] float m_translationSpeed = 2f;

    Transform m_rotation;
    Transform m_translation;

    [SerializeField] SoulTeleporter m_connectingTeleporter;

    Collider2D m_collider;
    private void Awake()
    {
        m_collider = GetComponent<Collider2D>();
        m_rotation = transform.GetChild(0);
        m_translation = m_rotation.transform.GetChild(0);
    }
    public void Spiral(Transform light)
    {
        StartCoroutine(SpiralCoroutine(light));
    }

    private IEnumerator SpiralCoroutine(Transform light)
    {
        float timeToKill = 2f;
        //m_connectingTeleporter.DeactivateCollider();
        light.GetComponent<Collider2D>().enabled = false;
        while (timeToKill > 0 && light)
        {
            timeToKill -= Time.deltaTime;
            m_translation.position = light.position;
            m_rotation.localEulerAngles += Vector3.forward * m_rotationSpeed * Time.deltaTime;
            m_translation.localPosition = Vector3.Lerp(m_translation.localPosition, Vector3.zero, m_translationSpeed * Time.deltaTime);
            light.position = m_translation.position;
            yield return null;
        }
        
        light.position = m_connectingTeleporter.transform.position;
        //Invoke(nameof(ReactivateCollider), 0.75f);
        light.GetComponent<PossessingLight>().SetFreeFromAttraction();
    }

    private void DeactivateCollider()
    {
        m_collider.enabled = false;
        
    }
    private void ReactivateCollider()
    {
        m_collider.enabled = true;
    }
}
