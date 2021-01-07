﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SpiderWeb : MonoBehaviour
{
    [SerializeField] float m_slowPercentage;

    Spider m_spider;

    private void Start()
    {
        m_spider = GetComponentInChildren<Spider>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                cmpDemon.Slow(m_slowPercentage);
                m_spider?.SetTarget(cmpDemon.transform);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out BasicZombie cmpDemon))
        {
            if (cmpDemon.IsControlledByPlayer)
            {
                cmpDemon.CancelSlow(m_slowPercentage);
                m_spider?.ReturnToInitialPosition();
            }
        }
    }
}
