using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerberus : MonoBehaviour
{

    [SerializeField] float m_speed = 3.5f;
    [SerializeField] float m_distanceToEat = 2f;
    [SerializeField] float m_offsetWaitWhenInterruptedByObstacle = 5f;
    [SerializeField] float m_obstacleDetectionHeight = 3f;

    [SerializeField] Transform[]    m_patrolTransforms;
    [SerializeField] Transform      m_mouthTransform;

    [SerializeField] SpriteRenderer [] m_mandibulas;

    [SerializeField] GameObject [] m_eyes;
    [SerializeField] float m_timeToEatCorpse = 0.5f;

    int     m_currentPatrolIndex = 0;
    bool    m_waiting;

    Rigidbody2D     m_RGB;
    Animator        m_animator;
    List<DemonBase> m_charactersInView;

    CerberusState   m_currentState = CerberusState.Patrol;
    Vector3[]       m_patrolPositions;


    private CerberusState CurrentState
    {
        get => m_currentState; set
        {
            if (value == CerberusState.Chasing)
            {
                for (int i = 0; i < m_eyes.Length; i++)
                {
                    m_eyes[i].SetActive(true);
                }
            }
            else if (value == CerberusState.Patrol)
            {
                for (int i = 0; i < m_eyes.Length; i++)
                {
                    m_eyes[i].SetActive(false);
                }
            }
            m_currentState = value;
        }
    }


    private enum CerberusState
    {
        Patrol, Chasing, Eating
    }

    private void Awake()
    {
        m_RGB = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_charactersInView = new List<DemonBase>();
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
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 origin = (Vector2)transform.position+Vector2.up*m_obstacleDetectionHeight- (Vector2)transform.right * transform.localScale.x * 5f;
        //Debug.DrawRay(origin, -transform.right * transform.localScale.x * 3f, Color.green);
        RaycastHit2D impact = Physics2D.Raycast(origin,-transform.right * transform.localScale.x,3f,1<<0);

        m_waiting = impact;

        if (!m_waiting)
        {
            if (CurrentState == CerberusState.Patrol)
            {
                Patrol();
            }
            else if (CurrentState == CerberusState.Chasing)
            {
                Chase();
            }
        }
        else
        {
            if (m_charactersInView.Count > 0)
            {
                int indexOfNearestDemon = 0;
                for (int i = 0; i < m_charactersInView.Count; i++)
                {
                    if (Vector3.Distance(transform.position, m_charactersInView[indexOfNearestDemon].Torso.transform.position) > Vector3.Distance(transform.position, m_charactersInView[i].Torso.transform.position))
                    {
                        indexOfNearestDemon = i;
                    }
                }

                //Vector3 positionToMoveTo = m_charactersInView[indexOfNearestDemon].Torso.transform.position;
                if (impact.distance > Vector3.Distance(origin, m_charactersInView[indexOfNearestDemon].Torso.transform.position))
                {
                    Chase();
                }
            }
        }

        m_animator.SetBool("Run", !m_waiting);
    }

    /// <summary>
    /// Patrulla entre los puntos designados
    /// </summary>
    private void Patrol()
    {

        transform.position = Vector3.MoveTowards(transform.position, m_patrolPositions[m_currentPatrolIndex], m_speed * Time.deltaTime);
        if (m_patrolPositions[m_currentPatrolIndex].x < transform.position.x)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Vector3.Distance(transform.position, m_patrolPositions[m_currentPatrolIndex]) < 0.25f)
        {
            m_currentPatrolIndex = (m_currentPatrolIndex + 1) % m_patrolPositions.Length;

        }
    }

    /// <summary>
    /// Perseguir al jugador en horizontal. Si no se llega por diferencia de alturas, se queda parado en su misma posicion en X
    /// </summary>
    private void Chase()
    {

        int indexOfNearestDemon = 0;

        for (int i = 0; i < m_charactersInView.Count; i++)
        {
            if (Vector3.Distance(transform.position, m_charactersInView[indexOfNearestDemon].Torso.transform.position) > Vector3.Distance(transform.position, m_charactersInView[i].Torso.transform.position))
            {
                indexOfNearestDemon = i;
            }
        }
        Vector3 positionToMoveTo = m_charactersInView[indexOfNearestDemon].Torso.transform.position;
        positionToMoveTo.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, m_speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, m_charactersInView[indexOfNearestDemon].Torso.position) < m_distanceToEat)
        {
            Eat(m_charactersInView[indexOfNearestDemon]);
        }
        else if (Mathf.Abs(transform.position.x - m_charactersInView[indexOfNearestDemon].Torso.position.x) < 3f)
        {
            m_waiting = true;
        }
    }

    /// <summary>
    /// Comienza la corrutina de comerse a un personaje
    /// </summary>
    /// <param name="characterToEat">Personaje al que comerse</param>
    private void Eat(DemonBase characterToEat)
    {
        CurrentState = CerberusState.Eating;
        StartCoroutine(EatCoroutine(characterToEat));
    }

    /// <summary>
    /// Se come al personaje (lo mata y luego lo destruye)
    /// </summary>
    /// <param name="characterToEat">Personaje al que comerse</param>
    /// <returns></returns>
    private IEnumerator EatCoroutine(DemonBase characterToEat)
    {
        m_animator.SetTrigger("Attack");
        float fixedAnimationTime = 0.5f;
        float animationSpeed = fixedAnimationTime/m_timeToEatCorpse;
        float previousAnimationSpeed = m_animator.speed;
        m_animator.speed = animationSpeed;
        characterToEat.CanMove = false;
        characterToEat.IsPossessionBlocked = true;
        characterToEat.Torso.GetComponent<Rigidbody2D>().isKinematic = true;
        m_mandibulas[0].sortingLayerName = "Foreground";
        m_mandibulas[1].sortingLayerName = "Foreground";
        yield return new WaitForSeconds(m_timeToEatCorpse);
        //yield return StartCoroutine(IdleEating(m_timeToEatCorpse));
        characterToEat.Die(true);
        float time = 0;
        float maxTime = 0.75f/animationSpeed;
        while (time < maxTime)
        {
            characterToEat.Torso.position = Vector3.MoveTowards(characterToEat.Torso.position, m_mouthTransform.position, Time.deltaTime * 8f);
            time += Time.deltaTime;
            yield return null;
        }

        m_mandibulas[0].sortingLayerName = "Default";
        m_mandibulas[1].sortingLayerName = "Default";
        m_charactersInView.Remove(characterToEat);
        Destroy(characterToEat.gameObject);
        m_animator.speed = previousAnimationSpeed;
        if (m_charactersInView.Count > 0)
        {
            CurrentState = CerberusState.Chasing;

        }
        else
        {
            CurrentState = CerberusState.Patrol;

        }
    }

    IEnumerator IdleEating(float time)
    {
        m_animator.SetTrigger("IdleEating");
        yield return new WaitForSeconds(m_timeToEatCorpse);
        m_animator.SetTrigger("EndAttack");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        if (cmpDemon && !m_charactersInView.Contains(cmpDemon))
        {
            m_charactersInView.Add(cmpDemon);
            //si es el primero que añado, es que antes estaba en patrulla y tengo que empezar a perseguir
            if (m_charactersInView.Count == 1 && CurrentState != CerberusState.Eating)
            {
                CurrentState = CerberusState.Chasing;

            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        if (cmpDemon && m_charactersInView.Contains(cmpDemon))
        {
            m_charactersInView.Remove(cmpDemon);
            if (m_charactersInView.Count == 0 && CurrentState != CerberusState.Eating)
            {
                CurrentState = CerberusState.Patrol;

            }
        }
    }
}
