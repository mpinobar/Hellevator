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
    private Vector3 m_lastPosition;
    private float m_lastMovedHorizontalDelta;

    private List<DemonBase> m_enemiesOnPreassurePlate;
    [SerializeField] private LayerMask m_enemyLayerMask;
    List<SpikesWeightData> m_spikesData;

    private void Awake()
    {
        m_enemiesOnPreassurePlate = new List<DemonBase>();
        m_spikesData = new List<SpikesWeightData>();
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
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
       
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();

        if (cmpDemon != null)
        {
            bool isCounted = false;

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    isCounted = true;

                    //add the collider to the associated demon's collider list if it isnt already included
                    if (!m_spikesData[i].Colliders.Contains(collision.collider) && collision.gameObject.tag != "BodyCollider")
                    {
                        m_spikesData[i].Colliders.Add(collision.collider);
                    }
                }
            }
            if (!isCounted)
            {

                m_spikesData.Add(new SpikesWeightData(cmpDemon, collision.collider));
                m_enemiesOnPreassurePlate.Add(cmpDemon);
                cmpDemon.transform.parent = transform;

            }

        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        if (cmpDemon != null && collision.gameObject.tag != "BodyCollider")
        {

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    //remove the collider from the associated demon's collider list 
                    if (m_spikesData[i].Colliders.Contains(collision.collider))
                    {
                        m_spikesData[i].Colliders.Remove(collision.collider);

                        //all the limbs have exited the spikes
                        if (m_spikesData[i].Colliders.Count == 0)
                        {                            
                            m_spikesData.RemoveAt(i);
                            m_enemiesOnPreassurePlate.Remove(cmpDemon);
                            cmpDemon.transform.parent = null;
                        }
                        else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
                        {
                            m_enemiesOnPreassurePlate.Remove(cmpDemon);                            
                            m_spikesData.RemoveAt(i);
                            cmpDemon.transform.parent = null;
                        }
                    }
                }
            }
        }
    }
    
}
