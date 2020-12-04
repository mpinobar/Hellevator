using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalPeriodicPlatform : MonoBehaviour
{
    [SerializeField] Transform m_endPosition;
    [SerializeField] float m_period = 2f;
    [SerializeField] float m_waitTimeOnArrival = 0.25f;

    private Vector3 m_initialPosition;
    private Vector3 m_endPos;
    private bool    m_returningToInitialPosition;
    private float   m_waitTimer;
    //private Vector3 m_lastPosition;
    private float   m_speed;

    private List<DemonBase> m_enemiesOnPreassurePlate;
    [SerializeField] private LayerMask m_playerLayer = 1<<10;
    [SerializeField] private LayerMask m_bodyLayer = 1<<8;
    //List<SpikesWeightData> m_spikesData;

    public LayerMask PlayerLayer { get => m_playerLayer; set => m_playerLayer = value; }
    public LayerMask BodyLayer { get => m_bodyLayer; set => m_bodyLayer = value; }

    private void Awake()
    {
        m_enemiesOnPreassurePlate = new List<DemonBase>();
        //m_spikesData = new List<SpikesWeightData>();
        if (m_endPosition)
        {
            m_endPos = m_endPosition.position;
            m_waitTimer = m_waitTimeOnArrival;
            m_initialPosition = transform.position;
            m_returningToInitialPosition = false;
            m_speed = Vector2.Distance(m_initialPosition, m_endPos) / m_period;
        }

    }

    // Update is called once per frame
    void Update()
    {


        //if (Input.GetKeyDown(KeyCode.Y) && m_spikesData.Count > 0)
        //{
        //    for (int i = 0; i < m_spikesData.Count; i++)
        //    {
        //        for (int j = 0; j < m_spikesData[i].Colliders.Count; j++)
        //        {
        //            print(m_spikesData[i].Colliders[j].name);
        //        }
        //    }
        //}



        if (m_endPosition)
        {
            if (m_returningToInitialPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_initialPosition, m_speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, m_initialPosition) < 0.001f)
                {
                    m_waitTimer -= Time.deltaTime;
                    if (m_waitTimer <= 0)
                    {
                        m_returningToInitialPosition = false;
                        m_waitTimer = m_waitTimeOnArrival;
                    }
                }
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, m_endPos, m_speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, m_endPos) < 0.001f)
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

        for (int i = 0; i < m_enemiesOnPreassurePlate.Count; i++)
        {
            m_enemiesOnPreassurePlate[i].transform.localScale = new Vector3(m_enemiesOnPreassurePlate[i].transform.localScale.x / m_enemiesOnPreassurePlate[i].transform.lossyScale.x,
                                                                            m_enemiesOnPreassurePlate[i].transform.localScale.y / m_enemiesOnPreassurePlate[i].transform.lossyScale.y,1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();

        if (cmpDemon != null)
        {
            RaycastHit2D hit = Physics2D.Raycast(cmpDemon.Torso.position, Vector2.down, 2f, 1<<0);
            if (hit.transform != null && (hit.transform == transform || hit.transform == transform.GetChild(0)))
            {
                //bool isCounted = false;

                //if (m_spikesData == null)
                //{
                //    m_spikesData = new List<SpikesWeightData>();
                //}

                //for (int i = 0; i < m_spikesData.Count; i++)
                //{
                //    //if the demon is already inside the spikes
                //    if (cmpDemon == m_spikesData[i].AssociatedDemon)
                //    {
                //        isCounted = true;

                //        //add the collider to the associated demon's collider list if it isnt already included
                //        if (!m_spikesData[i].Colliders.Contains(collision.collider) && collision.gameObject.tag != "BodyCollider")
                //        {
                //            m_spikesData[i].Colliders.Add(collision.collider);
                //        }
                //    }
                //}
                //if (!isCounted)
                //{

                //    m_spikesData.Add(new SpikesWeightData(cmpDemon, collision.collider));
                    m_enemiesOnPreassurePlate.Add(cmpDemon);
                    cmpDemon.transform.parent = transform;

                //}
            }


        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;

        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        if (cmpDemon != null)
        {

            //for (int i = 0; i < m_spikesData.Count; i++)
            //{
            //    //if the demon is already inside the spikes
            //    if (cmpDemon == m_spikesData[i].AssociatedDemon)
            //    {

            //        if (cmpDemon.IsControlledByPlayer)
            //        {
            //            m_spikesData.RemoveAt(i);
            //            m_enemiesOnPreassurePlate.Remove(cmpDemon);
            //            cmpDemon.transform.parent = null;
            //            return;
            //        }


            //        //remove the collider from the associated demon's collider list 
            //        if (m_spikesData[i].Colliders.Contains(collision.collider))
            //        {
            //            m_spikesData[i].Colliders.Remove(collision.collider);

            //            //all the limbs have exited the spikes
            //            if (m_spikesData[i].Colliders.Count == 0)
            //            {
            //                m_spikesData.RemoveAt(i);
            //                m_enemiesOnPreassurePlate.Remove(cmpDemon);
            //                cmpDemon.transform.parent = null;
            //            }
            //            else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].gameObject.layer == m_bodyLayer)
            //            {
            //                m_enemiesOnPreassurePlate.Remove(cmpDemon);
            //                m_spikesData.RemoveAt(i);
            //                cmpDemon.transform.parent = null;
            //            }
            //        }
            //    }
            //}


            if (m_enemiesOnPreassurePlate.Contains(cmpDemon) /*cmpDemon.IsControlledByPlayer*/)
            {
                m_enemiesOnPreassurePlate.Remove(cmpDemon);
                cmpDemon.transform.parent = null;
                return;
            }


        }
    }
}
