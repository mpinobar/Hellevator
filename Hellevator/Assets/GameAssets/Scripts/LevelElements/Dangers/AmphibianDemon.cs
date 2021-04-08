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
    Animator m_animator;

    [SerializeField] FrogWater m_detectionTrigger;

    float m_initialScale;
    private void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
        StartCoroutine(Patrol());
             
    }
    private void OnEnable()
    {
        m_initialScale = transform.localScale.x;
        m_detectionTrigger.Active = true;
        m_detectionTrigger.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        m_detectionTrigger.Active = false;
        if(m_detectionTrigger)
        m_detectionTrigger.gameObject.SetActive(false);
    }

    private IEnumerator Chase()
    {
        //Debug.LogError("Starting chase");
        float time = 0f;
        Vector3 offsetPosition = m_target.position - Vector3.up*m_verticalOffsetChase;
        while (Vector2.Distance(transform.position, offsetPosition) > 2.5f)
        {
            offsetPosition = m_target.position - Vector3.up * m_verticalOffsetChase;
            offsetPosition.y = Mathf.Min(4, offsetPosition.y);
            if(offsetPosition.x < transform.position.x)
            {
                transform.localScale = Vector3.one * m_initialScale;
                transform.localEulerAngles = Vector3.one - Vector3.forward + Vector3.forward * 81f;
            }
            else
            {
                transform.localScale = (Vector3.one - Vector3.up * 2) * m_initialScale;
                transform.localEulerAngles = Vector3.one - Vector3.forward + Vector3.forward * 97f;
            }
            transform.position += (offsetPosition - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            //transform.right = Vector3.Lerp(transform.right, (transform.position- offsetPosition), m_rotationSpeed * Time.deltaTime);
            yield return null;
        }
        float rot = transform.localEulerAngles.z;
        while (time < m_timeBeforeJumping)
        {
            time += Time.deltaTime;
            offsetPosition = m_target.position - Vector3.up * m_verticalOffsetChase;
            offsetPosition.y = Mathf.Min(4, offsetPosition.y);
            transform.position += (offsetPosition - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            if(transform.localScale.y < 0)
            {
                rot = Mathf.Lerp(rot, -177.7f, m_rotationSpeed * Time.deltaTime);
            }
            else
            {
                rot = Mathf.Lerp(rot, -4.7f, m_rotationSpeed * Time.deltaTime);

            }
            transform.localEulerAngles = Vector3.forward * rot;
            
            //transform.right = Vector3.Lerp(transform.right, Vector3.right, m_rotationSpeed * Time.deltaTime);
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
        m_patrolIndex = 1;
        m_detectionTrigger.Active = false;
        m_detectionTrigger.gameObject.SetActive(false);
        ReturnToPatrol();
    }


    private IEnumerator Patrol()
    {
        while (true)
        {
            transform.position += (m_patrolPoints[m_patrolIndex].position - transform.position).normalized * m_movementSpeed * Time.deltaTime;
            if(m_patrolPoints[m_patrolIndex].position.x < transform.position.x)
            {
                transform.localScale = Vector3.one*m_initialScale;
                transform.localEulerAngles = Vector3.one - Vector3.forward + Vector3.forward * 81f;
            }
            else
            {
                transform.localScale = (Vector3.one - Vector3.up * 2)*m_initialScale;
                transform.localEulerAngles = Vector3.one - Vector3.forward + Vector3.forward * 97f;
            }
            if (Vector2.Distance(transform.position, m_patrolPoints[m_patrolIndex].position) < 0.5f)
            {
                gameObject.SetActive(false);

                //m_patrolIndex = (m_patrolIndex + 1) % m_patrolPoints.Count;
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
                m_detectionTrigger.Active = false;
                ReturnToPatrol();
            }
        }
    }
}
