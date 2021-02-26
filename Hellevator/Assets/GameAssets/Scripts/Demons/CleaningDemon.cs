using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CleaningDemon : MonoBehaviour
{
    Rigidbody2D m_RGB;

    [SerializeField] float m_patrolSpeed = 3.5f;
    [SerializeField] float m_chargeSpeed = 5f;

   

    [SerializeField] Transform[] m_patrolTransforms;
    [SerializeField] float m_timeToStartCharging = 2f;
    float m_getAngryTimer;
    bool m_gettingAngry;
    int m_currentPatrolIndex = 0;
    Vector3[] m_patrolPositions;
    CleaningDemonState m_currentState = CleaningDemonState.Patrol;
    Animator m_animator;

    [SerializeField] GameObject [] m_eyes;
    [SerializeField] float m_timeStunned = 4f;
    float m_stunnedTimer;

    bool m_stunned;
    [SerializeField] DestructionCart m_cart;

    private CleaningDemonState CurrentState
    {
        get => m_currentState; set
        {
            if (value == CleaningDemonState.Charging)
            {
                for (int i = 0; i < m_eyes.Length; i++)
                {
                    m_eyes[i].SetActive(true);
                }
                m_cart.m_active = true;
            }
            else if (value == CleaningDemonState.Patrol)
            {
                for (int i = 0; i < m_eyes.Length; i++)
                {
                    m_eyes[i].SetActive(false);
                }
                m_cart.m_active = false;
            }
            m_currentState = value;
        }
    }


    private enum CleaningDemonState
    {
        Patrol, Charging
    }

    private void Awake()
    {
        m_RGB = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_stunnedTimer = m_timeStunned;
        m_getAngryTimer = m_timeToStartCharging;        
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < m_patrolTransforms.Length; i++)
        {
            m_patrolTransforms[i].position = new Vector3(m_patrolTransforms[i].position.x, transform.position.y, 0);
        }

        m_patrolPositions = new Vector3[m_patrolTransforms.Length];

        for (int i = 0; i < m_patrolPositions.Length; i++)
        {
            m_patrolPositions[i] = m_patrolTransforms[i].position;
        }
        
        
        CurrentState = CleaningDemonState.Patrol;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.LogError(m_currentState.ToString());

        if (!m_stunned)
        {
            if (CurrentState == CleaningDemonState.Patrol)
            {
                if (m_gettingAngry)
                {
                    m_getAngryTimer -= Time.deltaTime;
                    if (m_getAngryTimer <= 0)
                    {
                        CurrentState = CleaningDemonState.Charging;
                        m_gettingAngry = false;
                        m_getAngryTimer = m_timeToStartCharging;
                        m_cart.m_active = true;
                    }
                }
                else
                {
                    m_getAngryTimer = m_timeToStartCharging;
                    Patrol();
                }
            }
            else if (CurrentState == CleaningDemonState.Charging)
            {
                Charge();
            }
        }
        else
        {
            m_stunnedTimer -= Time.deltaTime;
            if (m_stunnedTimer <= 0)
            {
                m_stunned = false;
                CurrentState = CleaningDemonState.Patrol;
            }
        }

        m_animator.SetBool("Run", !m_stunned);
    }

    private void Patrol()
    {

        transform.position = Vector3.MoveTowards(transform.position, m_patrolPositions[m_currentPatrolIndex], m_patrolSpeed * Time.deltaTime);
        if (m_patrolPositions[m_currentPatrolIndex].x < transform.position.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        if (Vector3.Distance(transform.position, m_patrolPositions[m_currentPatrolIndex]) < 0.25f)
        {
            m_currentPatrolIndex = (m_currentPatrolIndex + 1) % m_patrolPositions.Length;

        }
    }

    private void Charge()
    {
        Vector3 direction = transform.right*transform.localScale.x;

        transform.position += direction * Time.deltaTime * m_chargeSpeed;

    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character && character.IsControlledByPlayer)
        {
            m_gettingAngry = true;
        }        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character && character.IsControlledByPlayer)
        {
            m_gettingAngry = false;
            m_getAngryTimer = 0f;
        }
    }

    public void Stop()
    {
        m_stunnedTimer = m_timeStunned;
        m_stunned = true;
        m_cart.m_active = false;
    }

}
