using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleLightSpeedModifier : MonoBehaviour
{
    [SerializeField] float m_speed;

    private void Start()
    {
        transform.GetChild(0).GetComponent<Animator>().speed = m_speed;
        transform.GetChild(0).GetChild(0).GetComponent<Animator>().speed = m_speed;
    }
}
