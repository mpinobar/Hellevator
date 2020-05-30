using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimacionPuerta : MonoBehaviour
{
    [SerializeField] Transform m_endPosition;
    [SerializeField] Vector3 m_startPosition;
    [SerializeField] float m_speed = 3f;

    private bool m_started;
    private bool m_closing;
    public bool Started { get => m_started; set => m_started = value; }
    public bool Closing { get => m_closing; set => m_closing = value; }

    private void Start()
    {
        m_startPosition = transform.localPosition;
    }


    // Update is called once per frame
    void Update()
    {
        if (m_started)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_endPosition.position, m_speed * Time.deltaTime);
        }
        else if(m_closing)
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_startPosition, m_speed * Time.deltaTime);
        }
    }
}
