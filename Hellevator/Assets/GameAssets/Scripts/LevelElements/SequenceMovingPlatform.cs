using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequenceMovingPlatform : ActivatedBase
{
    [SerializeField] float m_speed = 2f;
    [SerializeField] List <Transform> m_waypoints;
    List<Vector3> m_waypointPositions;
    private int m_currentIndex = 0;
    private int m_desiredIndex = 0;

    public override void Activate()
    {
        base.Activate();
        m_desiredIndex++;
        if (m_desiredIndex > m_waypoints.Count)
        {
            m_desiredIndex = m_waypoints.Count;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_waypointPositions = new List<Vector3>();
        m_waypointPositions.Add(transform.position);
        for (int i = 0; i < m_waypoints.Count; i++)
        {
            m_waypointPositions.Add(m_waypoints[i].position);
        }
    }

    // Update is called once per frame
    void Update()
    {        
        transform.position = Vector3.MoveTowards(transform.position, m_waypointPositions[m_currentIndex], m_speed * Time.deltaTime);
        if (Vector2.Distance(transform.position, m_waypointPositions[m_currentIndex]) < 0.2f)
        {
            transform.position = m_waypointPositions[m_currentIndex];
            //move current index closer to desired index
            if (m_currentIndex < m_desiredIndex)
            {
                m_currentIndex++;
            }
            else if (m_currentIndex > m_desiredIndex)
            {
                m_currentIndex--;
            }
        }
    }

    public override void Deactivate()
    {
        base.Deactivate();
        m_desiredIndex--;
        if (m_desiredIndex < 0)
        {
            m_desiredIndex = 0;
        }

    }

    
}
