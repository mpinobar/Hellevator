using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicMovement : MonoBehaviour
{
    [SerializeField] private Transform m_endPosition;
    private bool m_returningToInitialPosition;
    [SerializeField] float m_period;
    Vector3 m_initialPosition;
    float m_speed;
    float m_waitTimer;
    [SerializeField] float m_waitTimeOnArrival;
    Vector3 m_endPos;

    // Start is called before the first frame update
    void Start()
    {
        if (m_endPosition)
        {
            m_endPos = m_endPosition.position;
            m_waitTimer = m_waitTimeOnArrival;
            m_initialPosition = transform.position;
            m_returningToInitialPosition = false;
            m_speed = Vector2.Distance(m_initialPosition, m_endPos) / m_period;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_endPosition)
        {
            if (m_returningToInitialPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_initialPosition, m_speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, m_initialPosition) < 0.001f)
                {
                    m_waitTimer -= Time.deltaTime;
                    if (m_waitTimer <= 0)
                    {
                        m_returningToInitialPosition = false;
                        m_waitTimer = m_waitTimeOnArrival;
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_endPos, m_speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, m_endPos) < 0.001f)
                {
                    m_waitTimer -= Time.deltaTime;
                    if (m_waitTimer <= 0)
                    {
                        m_returningToInitialPosition = true;
                        m_waitTimer = m_waitTimeOnArrival;
                    }
                }
            }
        }
    }
}
