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
                                                                            m_enemiesOnPreassurePlate[i].transform.localScale.y / m_enemiesOnPreassurePlate[i].transform.lossyScale.y, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.LogError(LayerMask.LayerToName(collision.gameObject.layer));

        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        //Debug.LogError("In " + collision.gameObject.name);
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        //Debug.LogError(cmpDemon.name);
        if (cmpDemon != null && !m_enemiesOnPreassurePlate.Contains(cmpDemon))
        {
            RaycastHit2D hit = Physics2D.Raycast(cmpDemon.Torso.position, Vector2.down, 2f, 1<<0);
            if (hit.transform != null && (hit.transform == transform || hit.transform == transform.GetChild(0)))
            {
                m_enemiesOnPreassurePlate.Add(cmpDemon);
                cmpDemon.transform.parent = transform;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        //Debug.LogError("Out "+collision.gameObject.name);
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        if (cmpDemon != null)
        {

            if (m_enemiesOnPreassurePlate.Contains(cmpDemon) /*cmpDemon.IsControlledByPlayer*/)
            {
                m_enemiesOnPreassurePlate.Remove(cmpDemon);
                cmpDemon.transform.parent = null;
                if (cmpDemon.IsControlledByPlayer)
                {

                    Vector2 auxVelocity = cmpDemon.MyRgb.velocity;
                    int dir;
                    if (m_returningToInitialPosition)
                    {
                        if(transform.position.x > m_initialPosition.x)
                        {
                            dir = -1;
                        }
                        else if (transform.position.x < m_initialPosition.x)
                        {
                            dir = 1;
                        }
                        else
                        {
                            dir = 0;
                        }
                    }
                    else
                    {
                        if (transform.position.x > m_endPos.x)
                        {
                            dir = -1;
                        }
                        else if (transform.position.x < m_endPos.x)
                        {
                            dir = 1;
                        }
                        else
                        {
                            dir = 0;
                        }
                    }
                    //Debug.LogError(dir);
                    auxVelocity += Vector2.right * dir * m_speed;
                    //Debug.LogError("Aux velocity: " + auxVelocity);
                    //cmpDemon.MyRgb.velocity = auxVelocity;
                    StopAllCoroutines();
                    StartCoroutine(Inertia((BasicZombie)cmpDemon, auxVelocity.x));
                    //if (Mathf.Abs(auxVelocity.x) > Mathf.Abs(cmpDemon.MyRgb.velocity.x))
                    //{
                    //    cmpDemon.MyRgb.velocity = auxVelocity;
                    //}
                }
                return;
            }
        }
    }

    IEnumerator Inertia(BasicZombie character, float inertialVelocity)
    {
        float currentInertia = inertialVelocity;
        float totalDecayTime = .5f;
        float decayTime = totalDecayTime;

        while(decayTime > 0)
        {
            decayTime -= Time.deltaTime;
            currentInertia = inertialVelocity * (decayTime / totalDecayTime)*0.25f;
            //Debug.LogError(currentInertia * 0.25f);
            character.DragMovement(currentInertia);
            yield return null;
        }
        character.DragMovement(0);

    }

}
