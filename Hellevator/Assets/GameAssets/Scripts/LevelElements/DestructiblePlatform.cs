using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_decorado;
    [SerializeField] private float m_timeToReappear = 2f;
    [SerializeField] private float m_timeToDestroy;
    [SerializeField] private float m_shakeStrength;
    [SerializeField] private float m_multiplierAtEnd = 2;
    [SerializeField] private int m_framesPerShake;
    SpriteRenderer rnd;
    private bool m_destroying;
    private float m_currentMultiplier = 1;
    private Vector3 m_initPos;
    private float tmp;
    private float m_percentageOfDestruction;
    Collider2D m_collider;
    private float tmpReappear;
    private int tmpShake;
    private Vector2 m_targetShakePosition;
    private bool willReappear = true;

    LayerMask m_playerLayer;
    LayerMask m_bodyLayer;

    List<SpikesWeightData> m_spikesData;
    private List<DemonBase> m_enemiesOnPreassurePlate;

    public bool WillReappear { get => willReappear; set => willReappear = value; }

    private bool m_isParentMovingPlatform;

    // Start is called before the first frame update
    void Start()
    {
        m_collider = GetComponent<Collider2D>();
        rnd = GetComponent<SpriteRenderer>();
        if (m_decorado)
        {
            m_initPos = m_decorado.transform.position;
        }
        tmp = m_timeToDestroy;
        tmpShake = m_framesPerShake;
        tmpReappear = m_timeToReappear;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_destroying)
        {
            tmp -= Time.deltaTime;
            m_percentageOfDestruction = tmp / m_timeToDestroy;
            Color c = rnd.color;
            c.a = m_percentageOfDestruction;
            rnd.color = c;
            if (m_decorado)
            {
                m_decorado.color = c;
                m_currentMultiplier = (1 - m_percentageOfDestruction) * m_multiplierAtEnd;
                if (tmpShake == m_framesPerShake)
                {
                    m_targetShakePosition = new Vector2(m_initPos.x + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f * m_currentMultiplier, m_initPos.y + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f * m_currentMultiplier);
                }
                tmpShake--;
                m_decorado.transform.position = Vector2.MoveTowards(m_decorado.transform.position, m_targetShakePosition, m_currentMultiplier * Time.deltaTime);
                if (tmpShake <= 0)
                {
                    tmpShake = m_framesPerShake;
                }

            }
            if (tmp <= 0)
            {
                m_collider.enabled = false;
                m_destroying = false;
            }
        }
        else if (willReappear)
        {
            if (tmp <= 0)
            {
                tmpReappear -= Time.deltaTime;
                if (tmpReappear <= 0)
                {
                    tmp = m_timeToDestroy;
                    tmpReappear = m_timeToReappear;
                    m_collider.enabled = true;
                    Color c = rnd.color;
                    c.a = 1;
                    rnd.color = c;
                    if (m_decorado)
                    {
                        m_decorado.color = c;
                        m_decorado.transform.position = m_initPos;
                    }
                }
            }
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        DemonBase demon = collision.transform.root.GetComponent<DemonBase>();
        if (demon && demon.IsControlledByPlayer)
        {
            RaycastHit2D hit = Physics2D.Raycast(collision.transform.position,Vector2.down,0.5f,1 << 0);
            
            if (hit.transform != null && hit.transform == transform)
            {
                m_destroying = true;
            }
        }
        else
        {
            if (!willReappear)
            {
                GetComponent<Rigidbody2D>().isKinematic = true;
                GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezePosition;
                HorizontalPeriodicPlatform hpp = collision.transform.GetComponent<HorizontalPeriodicPlatform>();
                if (hpp)
                {                   
                    m_bodyLayer = hpp.BodyLayer;
                    m_playerLayer = hpp.PlayerLayer;
                    transform.parent = collision.transform;
                    m_isParentMovingPlatform = true;
                }
            }
        }


        if (m_isParentMovingPlatform)
        {
            DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();

            if (cmpDemon != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(cmpDemon.Torso.position, Vector2.down, 2f, 1<<0);
                if (hit.transform != null && (hit.transform == transform || hit.transform == transform.GetChild(0)))
                {
                    bool isCounted = false;

                    if (m_spikesData == null)
                    {
                        m_spikesData = new List<SpikesWeightData>();
                    }
                    if(m_enemiesOnPreassurePlate == null)
                    {
                        m_enemiesOnPreassurePlate = new List<DemonBase>();
                    }

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
                        cmpDemon.transform.parent = transform.parent;

                    }
                }


            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (m_isParentMovingPlatform)
        {
            DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
            if (cmpDemon != null)
            {

                for (int i = 0; i < m_spikesData.Count; i++)
                {
                    //if the demon is already inside the spikes
                    if (cmpDemon == m_spikesData[i].AssociatedDemon)
                    {

                        if (cmpDemon.IsControlledByPlayer)
                        {
                            m_spikesData.RemoveAt(i);
                            m_enemiesOnPreassurePlate.Remove(cmpDemon);
                            cmpDemon.transform.parent = null;
                            return;
                        }


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
                            else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].gameObject.layer == m_bodyLayer)
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
}
