using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRotation : MonoBehaviour
{
    [SerializeField] float m_period = 2f;
    float m_frecuency;
    float m_angularSpeed;
    [SerializeField] float m_maxAngle = 30f;
    bool m_isClockwise;
    Quaternion m_targetRotation;

    private void Awake()
    {
        m_angularSpeed = m_maxAngle * 0.5f / m_period;
    }

    private void Start()
    {
        m_targetRotation = Quaternion.Euler(Vector3.forward * m_maxAngle);/*Quaternion.FromToRotation(transform.eulerAngles, Vector3.forward * m_maxAngle);*/
    }

    // Update is called once per frame
    void Update()
    {        
        if (Mathf.Abs(transform.eulerAngles.z - m_maxAngle) < 1.5f || Mathf.Abs(360 - transform.eulerAngles.z - m_maxAngle) < 1.5f)
        {
            if (m_isClockwise)
            {
                m_targetRotation = Quaternion.Euler(Vector3.forward * m_maxAngle);

            }
            else
            {
                m_targetRotation = Quaternion.Euler(-Vector3.forward * m_maxAngle);
            }
            m_isClockwise = !m_isClockwise;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, m_targetRotation, m_angularSpeed * Time.deltaTime);
    }
}
