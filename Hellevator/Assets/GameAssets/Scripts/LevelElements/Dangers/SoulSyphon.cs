﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSyphon : MonoBehaviour
{
    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    DemonBase character = collision.GetComponentInParent<DemonBase>();
    //    if (character)
    //    {
    //        character.IsPossessionBlocked = true;
    //    }
    //}
    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    DemonBase character = collision.GetComponentInParent<DemonBase>();
    //    if (character)
    //    {
    //        character.IsPossessionBlocked = false;
    //    }
    //}

    [SerializeField] float m_rotationSpeed = 540f;
    [SerializeField] float m_translationSpeed = 2f;

    Transform m_rotation;
    Transform m_translation;


    private void Awake()
    {
        m_rotation = transform.GetChild(0);
        m_translation= m_rotation.transform.GetChild(0);
    }
    public void Spiral(Transform light)
    {
        StartCoroutine(SpiralCoroutine(light));
    }

    private IEnumerator SpiralCoroutine(Transform light)
    {
        float timeToKill = 2f;
        while (timeToKill > 0 && light)
        {
            timeToKill -= Time.deltaTime;
            m_translation.position = light.position;
            m_rotation.localEulerAngles += Vector3.forward * m_rotationSpeed * Time.deltaTime;
            m_translation.localPosition = Vector3.Lerp(m_translation.localPosition, Vector3.zero, m_translationSpeed * Time.deltaTime);
            light.position = m_translation.position;
            yield return null;
        }
        light.GetComponent<PossessingLight>().KillLightAndRestart();
    }
}