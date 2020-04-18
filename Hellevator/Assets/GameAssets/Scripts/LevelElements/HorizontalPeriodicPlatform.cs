using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPeriodicPlatform : MonoBehaviour
{
    [SerializeField] Transform m_endPosition;
    [SerializeField] float m_speed = 1f;
    [SerializeField] float m_waitTimeOnArrival = 0.25f;

    private Vector3 m_initialPosition;
    private Vector3 m_endPos;
    private bool m_returningToInitialPosition;
    private float m_waitTimer;

    private void Awake()
    {
        m_endPos = m_endPosition.position;
        m_waitTimer = m_waitTimeOnArrival;
        m_initialPosition = transform.position;
        m_returningToInitialPosition = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_returningToInitialPosition)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_initialPosition, m_speed * Time.deltaTime);

            if(Vector2.Distance(transform.position, m_initialPosition) < 0.1f)
            {
                m_waitTimer -= Time.deltaTime;
                if(m_waitTimer <= 0)
                {
                    m_returningToInitialPosition = false;
                    m_waitTimer = m_waitTimeOnArrival;
                }
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position,m_endPos, m_speed * Time.deltaTime);

            if (Vector2.Distance(transform.position, m_endPos) < 0.1f)
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
