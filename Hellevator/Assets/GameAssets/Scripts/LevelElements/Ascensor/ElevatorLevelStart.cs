using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorLevelStart : MonoBehaviour
{
    [SerializeField] Transform m_endPosition;
    [SerializeField] float m_speed = 4f;
    [SerializeField] AnimacionPuerta m_leftDoor;
    [SerializeField] AnimacionPuerta m_rightDoor;
    bool m_travelling;

    public bool Travelling { get => m_travelling; set => m_travelling = value; }

    // Start is called before the first frame update
    void Start()
    {
        m_travelling = true;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_endPosition.position, m_speed * Time.deltaTime);
            if(Vector3.Distance(transform.position,m_endPosition.position) < 0.2f)
            {
                m_leftDoor.Started = true;
                m_rightDoor.Started = true;
                m_travelling = false;
            }
        }
    }

    public void CloseDoors()
    {
        m_leftDoor.Started = false;
        m_leftDoor.Closing = true;
        m_rightDoor.Started = false;
        m_rightDoor.Closing = true;
    }
}
