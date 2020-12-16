using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroParallax : MonoBehaviour
{
    [SerializeField] AnimationCurve m_verticalCurve;
    [SerializeField] Transform m_endLocation;
    [SerializeField] float m_speed;

    float m_time;
    Vector3 m_initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        m_initialPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_time <= 1)
            m_time += Time.deltaTime * m_speed;
        Mathf.Clamp01(m_time);
        transform.position = m_initialPosition + (m_endLocation.position - m_initialPosition) * m_verticalCurve.Evaluate(m_time);
    }
}
