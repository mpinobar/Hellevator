using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Liana : MonoBehaviour
{
    [SerializeField] float m_delayToRelease = 1;
    [SerializeField] float m_rotationSpeed = 30;
    [SerializeField] float m_characterVerticalOffsetOnGrabbed = 2f;
    [SerializeField] float m_characterFollowSpeed = 8f;
    //[SerializeField] Transform m_visual;
    [SerializeField] Transform m_colliderTransform;
    [SerializeField] Transform m_collidedPosition;

    BasicZombie m_character;

    float m_initialEulerZValue;
    float m_endAngle;
    bool m_characterGrabbed;
    private void Start()
    {
        //m_initialEulerZValue = m_visual.localEulerAngles.z;
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
                StartCoroutine(DragDemon(cmpDemon));
            }
        }
    }

    private void GrabDemon(BasicZombie cmpDemon)
    {
        cmpDemon.SetOnLadder(true);
        //cmpDemon.CanMove = false;
        cmpDemon.MyRgb.isKinematic = true;
        m_characterGrabbed = true;
    }

    private IEnumerator DragDemon(BasicZombie cmpDemon)
    {
        //float time = 0;
        //m_endAngle = m_delayToRelease * m_rotationSpeed;
        m_collidedPosition.position = cmpDemon.transform.position;
        m_collidedPosition.localPosition -= Vector3.right * m_collidedPosition.localPosition.x;
        m_collidedPosition.localPosition += transform.up * 0.15f;
        float maxYPosition = Mathf.Max(m_collidedPosition.transform.localPosition.y,-9.5f);
        m_collidedPosition.localPosition = new Vector3(m_collidedPosition.localPosition.x, maxYPosition, 0);

        while (m_characterGrabbed)
        {
            //time += Time.deltaTime;
            //m_visual.localEulerAngles += Vector3.forward * m_rotationSpeed * Time.deltaTime;
            cmpDemon.transform.position = Vector3.Lerp(cmpDemon.transform.position, m_collidedPosition.position - Vector3.up * m_characterVerticalOffsetOnGrabbed, Time.deltaTime * m_characterFollowSpeed);
            yield return null;
        }

        //ReleaseDemon();
    }

    private void ReleaseDemon()
    {
        StopAllCoroutines();
        m_character.OnJumped -= ReleaseDemon;
        m_character.SetOnLadder(false);
        m_character.CanMove = true;
        m_characterGrabbed = false;
        m_character.MyRgb.isKinematic = false;
        m_colliderTransform.GetComponent<Collider2D>().enabled = false;
        StartCoroutine(ReturnToOrigin());
    }

    IEnumerator ReturnToOrigin()
    {
        //while (Mathf.Abs(m_visual.localEulerAngles.z - m_initialEulerZValue) > 2)
        //{
        //    m_visual.localEulerAngles -= Vector3.forward * m_rotationSpeed * Time.deltaTime;
        //    yield return null;
        //}
        yield return new WaitForSeconds(0.25f);
        m_colliderTransform.GetComponent<Collider2D>().enabled = true;
    }
    private void Update()
    {
        if (m_characterGrabbed && m_character.IsDead)
        {
            ReleaseDemon();
        }
    }
}
