using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightSpeedModifier : MonoBehaviour
{
    [SerializeField] float m_speed;
    [SerializeField] bool m_usesRange;

    [SerializeField] float m_rangeMin = 1;
    [SerializeField] float m_rangeMax = 2.5f;

    private void Start()
    {
        if (m_usesRange)
        {
            m_speed = Random.Range(m_rangeMin, m_rangeMax);
        }
        transform.GetChild(0).GetComponent<Animator>().speed = m_speed;
        transform.GetChild(0).GetChild(0).GetComponent<Animator>().speed = m_speed;
    }
}
