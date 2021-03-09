using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmphibianDemon : MonoBehaviour
{
    [SerializeField] List<Transform> m_patrolPoints;
    int m_patrolIndex;
    [SerializeField] float m_verticalOffsetChase = 10f;
    [SerializeField] float m_timeBeforeJumping = 2f;
    [SerializeField] float m_movementSpeed = 1f;
    [SerializeField] float m_rotationSpeed = 4f;
    [SerializeField] AnimationCurve m_heightCurve;
    [SerializeField] float m_maxHeight;
    [SerializeField] float m_jumpSpeed;
    Transform m_target;

    private void Start()
    {
        StartCoroutine(Patrol());
    }

    private IEnumerator Chase()
    {
        //Debug.LogError("Starting chase");
        float time = 0f;
        Vector3 offsetPosition = m_target.position - Vector3.up*m_verticalOffsetChase;
        while (Vector2.Distance(transform.position, offsetPosition) > 0.5f)
        {
            offsetPosition = m_target.position - Vector3.up * m_verticalOffsetChase;
            transform.position += (offsetPosition - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, (offsetPosition - transform.position), m_rotationSpeed * Time.deltaTime);
            yield return null;
        }

        while (time < m_timeBeforeJumping)
        {
            time += Time.deltaTime;
            offsetPosition = m_target.position - Vector3.up * m_verticalOffsetChase;
            transform.position += (offsetPosition - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, Vector3.up, m_rotationSpeed * Time.deltaTime);
            yield return null;
        }
        float animTime = 0;
        //Vector3 endPosition = offsetPosition + Vector3.up*m_maxHeight;
        while (animTime < 1)
        {
            animTime += Time.deltaTime * m_jumpSpeed;
            transform.position = offsetPosition + Vector3.up * m_maxHeight * m_heightCurve.Evaluate(animTime);
            yield return null;
        }
        StartCoroutine(Chase());
    }


    private IEnumerator Patrol()
    {
        while (true)
        {
            transform.position += (m_patrolPoints[m_patrolIndex].position - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            transform.up = (m_patrolPoints[m_patrolIndex].position - transform.position).normalized;
            if (Vector2.Distance(transform.position, m_patrolPoints[m_patrolIndex].position) < 0.5f)
            {
                m_patrolIndex = (m_patrolIndex + 1) % m_patrolPoints.Count;
            }
            yield return null;
        }
    }

    public void StartChase(Transform target)
    {
        m_target = target;
        StopAllCoroutines();
        StartCoroutine(Chase());
    }

    public void ReturnToPatrol()
    {
        m_target = null;
        StopAllCoroutines();
        StartCoroutine(Patrol());
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                cmpDemon.Die(true);
                ReturnToPatrol();
            }
        }
    }
}
