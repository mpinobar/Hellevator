using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caldero : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_spr;
    [SerializeField] float m_timeToExplode = 5f;
    float m_explodeTimer;
    [SerializeField] GameObject m_explosionPrefab;
    private bool m_covered = false;
    float m_changeSpeed;
    [SerializeField] Fire m_cauldronFire;
    bool canExplode = true;
    private void Awake()
    {
        m_changeSpeed = 1 / m_timeToExplode;
    }

    private void Update()
    {
        m_covered = m_cauldronFire.m_fireCovered;
        if (m_covered)
            if (!canExplode)
                m_covered = false;
        if (m_covered)
        {
            if (m_explodeTimer < m_timeToExplode)
                m_explodeTimer += Time.deltaTime;
            else if (canExplode)
                Explode();
            m_spr.color = Color.Lerp(m_spr.color, Color.red, Time.deltaTime * m_changeSpeed);
        }
        else
        {
            if (m_explodeTimer > 0)
                m_explodeTimer -= Time.deltaTime;
            else
                canExplode = true;
            m_spr.color = Color.Lerp(m_spr.color, Color.white, Time.deltaTime * m_changeSpeed);
        }
    }



    public void Explode()
    {
        Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
        canExplode = false;
    }
        
}
