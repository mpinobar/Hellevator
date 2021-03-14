using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonoBehaviour
{
    [SerializeField] float m_spiderSpeed = 1f;
    Transform m_target;
    [SerializeField] float m_rotationSpeed = 30f;
    Vector3 m_initialPosition;
    Quaternion m_initialRotation;

    // Start is called before the first frame update
    void Start()
    {
        m_initialPosition = transform.position;
        m_initialRotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_target)
        {
            transform.position += (m_target.position - transform.position).normalized * m_spiderSpeed * Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, (m_target.position - transform.position), m_rotationSpeed * Time.deltaTime);
        }
    }

    public void ReturnToInitialPosition()
    {
        StartCoroutine(ReturnToInitialPositionAndRotate());
    }

    IEnumerator ReturnToInitialPositionAndRotate()
    {
        m_target = null;

        while (Vector3.Distance(m_initialPosition, transform.position) > 2f)
        {
            transform.position += (m_initialPosition - transform.position).normalized * m_spiderSpeed * Time.deltaTime;
            transform.up = Vector3.Lerp(transform.up, (m_initialPosition - transform.position), m_rotationSpeed * Time.deltaTime);
            yield return null;
        }

        while (Quaternion.Angle(m_initialRotation, transform.rotation) > 1)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, m_initialRotation, m_rotationSpeed * Time.deltaTime);
            yield return null;
        }

    }

    public void SetTarget(Transform newTarget)
    {
        StopAllCoroutines();
        m_target = newTarget;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (!cmpDemon.IsDead)
            {
                m_target = null;
                cmpDemon.Die(true);
                ReturnToInitialPosition();
            }
        }
    }
}
