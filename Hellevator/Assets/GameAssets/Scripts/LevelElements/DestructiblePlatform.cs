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
                if(tmpShake == m_framesPerShake)
                {
                    m_targetShakePosition = new Vector2(m_initPos.x + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f*m_currentMultiplier, m_initPos.y + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f * m_currentMultiplier);
                }
                tmpShake--;
                m_decorado.transform.position = Vector2.MoveTowards(m_decorado.transform.position, m_targetShakePosition, m_currentMultiplier * Time.deltaTime);
                if(tmpShake <= 0)
                {
                    tmpShake = m_framesPerShake;
                }

            }
            if(tmp <= 0)
            {
                m_collider.enabled = false;
                m_destroying = false;
            }
        }
        else
        {
            if(tmp <= 0)
            {
                tmpReappear -= Time.deltaTime;
                if(tmpReappear <= 0)
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
            m_destroying = true;
        }
    }
}
