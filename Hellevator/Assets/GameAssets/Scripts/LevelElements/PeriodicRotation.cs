using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeriodicRotation : MonoBehaviour
{
    [SerializeField] float m_period;
    float m_angularSpeed;
    [SerializeField] float m_maxAngle = 30f;

    float time;
    private void Awake()
    {
        m_angularSpeed = 2f * Mathf.PI / m_period;
    }

    // Update is called once per frame
    void Update()
    {        
        time += Time.deltaTime;
        if (time >= m_period * 2f)
        {
            time = 0;
        }
        transform.eulerAngles = Vector3.forward * m_maxAngle * Mathf.Sin(m_angularSpeed * time);
    }
}
