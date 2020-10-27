﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class Shelter : MonoBehaviour
{
    [SerializeField] Boss boss;
    [SerializeField] bool m_startsBoss;
    [SerializeField] List<Transform> m_attackPlaces;

    bool m_init;

    private void Awake()
    {
        if (!boss)
        {
            boss = FindObjectOfType<Boss>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
        {
            if (m_startsBoss)
            {
                if (!m_init)
                {
                    boss.Begin();
                    for (int i = 0; i < m_attackPlaces.Count; i++)
                    {
                        boss.ThrowKnife(m_attackPlaces[i], Random.Range(0, 0.5f), true);
                    }
                    m_init = true;
                }

            }
            boss.SetNotSeeingPlayer();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
        boss.SetSeeingPlayer();
    }

}