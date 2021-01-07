using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Liana : MonoBehaviour
{
    [SerializeField] float m_delayToRelease = 1;
    [SerializeField] float m_rotationSpeed = 30;
    [SerializeField] float m_characterVerticalOffsetOnGrabbed = 2f;
    [SerializeField] Transform m_visual;
    [SerializeField] Transform m_colliderTransform;

    BasicZombie m_character;

    float m_initialEulerZValue;
    float m_endAngle;
    private void Start()
    {
        m_initialEulerZValue = m_visual.localEulerAngles.z;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                m_character = cmpDemon;
                m_character.OnJumped += ReleaseDemon;
                GrabDemon(m_character);
                StartCoroutine(Animate(cmpDemon));
            }
        }
    }

    private static void GrabDemon(BasicZombie cmpDemon)
    {
        cmpDemon.SetOnLadder(true);
        //cmpDemon.CanMove = false;
        cmpDemon.MyRgb.isKinematic = true;
    }

    private IEnumerator Animate(BasicZombie cmpDemon)
    {
        float time = 0;
        m_endAngle = m_delayToRelease * m_rotationSpeed;
        while (time < m_delayToRelease)
        {
            time += Time.deltaTime;
            m_visual.localEulerAngles += Vector3.forward * m_rotationSpeed * Time.deltaTime;
            cmpDemon.transform.position = Vector3.Lerp(cmpDemon.transform.position, m_colliderTransform.position - Vector3.up * m_characterVerticalOffsetOnGrabbed, Time.deltaTime * 4.5f);
            yield return null;
        }

        ReleaseDemon();
    }

    private void ReleaseDemon()
    {
        StopAllCoroutines();
        m_character.OnJumped -= ReleaseDemon;
        m_character.SetOnLadder(false);
        m_character.CanMove = true;
        m_character.MyRgb.isKinematic = false;
        m_colliderTransform.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(ReturnToOrigin());
    }

    IEnumerator ReturnToOrigin()
    {
        while (Mathf.Abs(m_visual.localEulerAngles.z - m_initialEulerZValue) > 2)
        {
            m_visual.localEulerAngles -= Vector3.forward * m_rotationSpeed * Time.deltaTime;
            yield return null;
        }
        m_colliderTransform.GetComponent<Collider2D>().enabled = true;
    }
}
