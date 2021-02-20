﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cerberus : MonoBehaviour
{

    [SerializeField] float m_speed = 3.5f;
    [SerializeField] float m_distanceToEat = 2f;
    [SerializeField] float m_offsetWaitWhenInterruptedByObstacle = 5f;
    [SerializeField] Transform[] m_patrolTransforms;
    [SerializeField] Transform m_mouthTransform;

    int m_currentPatrolIndex = 0;
    Vector3[] m_patrolPositions;
    CerberusState m_currentState = CerberusState.Patrol;
    Rigidbody2D m_RGB;
    Animator m_animator;

    [SerializeField] SpriteRenderer [] m_mandibulas;

    List<DemonBase> m_charactersInView;
    [SerializeField] GameObject [] m_eyes;
    bool m_waiting;

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
        //Debug.DrawRay((Vector2)transform.position + Vector2.up, (-transform.right * transform.localScale.x) * 8f, Color.green);
        RaycastHit2D impact = Physics2D.Raycast((Vector2)transform.position+Vector2.up,-transform.right * transform.localScale.x,8f,1<<0);
        //if (impact)
        //{
        //    Debug.LogError(impact.transform.name);
        //}
        m_waiting = impact;
        //if (m_currentState == CerberusState.Patrol && m_waiting)
        //{

        //}

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

        m_animator.SetBool("Run", !m_waiting);
    }

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

    private void Eat(DemonBase characterToEat)
    {
        CurrentState = CerberusState.Eating;
        StartCoroutine(EatCoroutine(characterToEat));
    }

    private IEnumerator EatCoroutine(DemonBase characterToEat)
    {
        m_animator.SetTrigger("Attack");
        characterToEat.CanMove = false;
        characterToEat.IsPossessionBlocked = true;
        characterToEat.Torso.GetComponent<Rigidbody2D>().isKinematic = true;
        m_mandibulas[0].sortingLayerName = "Foreground";
        m_mandibulas[1].sortingLayerName = "Foreground";
        yield return new WaitForSeconds(0.5f);
        characterToEat.Die(true);
        float time = 0;
        while (time < 0.75f)
        {
            characterToEat.Torso.position = Vector3.MoveTowards(characterToEat.Torso.position, m_mouthTransform.position, Time.deltaTime * 8f);
            time += Time.deltaTime;
            yield return null;
        }

        m_mandibulas[0].sortingLayerName = "Default";
        m_mandibulas[1].sortingLayerName = "Default";
        m_charactersInView.Remove(characterToEat);
        Destroy(characterToEat.gameObject);
        if (m_charactersInView.Count > 0)
        {
            CurrentState = CerberusState.Chasing;

        }
        else
        {
            CurrentState = CerberusState.Patrol;

        }
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