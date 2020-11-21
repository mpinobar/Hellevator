﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : ButtonActivatedBase
{

    [SerializeField] GameObject m_projectileToShoot;
    [SerializeField] Transform m_direction;
    [SerializeField] float m_initialWaitTimeBeforeShooting = 0f;
    [SerializeField] float m_timeIntervalBetweenShots = 2f;
    [SerializeField] float m_projectileSpeed = 4f;
    [SerializeField] bool m_activatedWithButton;
    [ColorUsage(true,true)]
    [SerializeField] Color m_spriteColor;
    SpriteRenderer m_spriteCmp;

    int maxNumberOfProjectiles = 5;
    List<GameObject> projectilePool;

    bool m_active;


    // Start is called before the first frame update
    void Start()
    {
        m_spriteCmp = GetComponentInChildren<SpriteRenderer>();
        m_spriteCmp.color = m_spriteColor;
        
        projectilePool = new List<GameObject>();
        for (int i = 0; i < maxNumberOfProjectiles; i++)
        {
            projectilePool.Add(Instantiate(m_projectileToShoot, transform.position, Quaternion.identity, transform));
            projectilePool[i].transform.up = (m_direction.position - transform.position).normalized;
            projectilePool[i].SetActive(false);
        }
        if (!m_activatedWithButton)
        {
            Activate();
        }
        InvokeRepeating(nameof(ShootProjectile), m_initialWaitTimeBeforeShooting, m_timeIntervalBetweenShots);
    }

    private void ShootProjectile()
    {
        if (m_active)
        {
            for (int i = 0; i < projectilePool.Count; i++)
            {
                if (!projectilePool[i].activeSelf)
                {
                    projectilePool[i].transform.position = transform.position;
                    projectilePool[i].SetActive(true);
                    projectilePool[i].GetComponent<Projectile>().Speed = m_projectileSpeed;
                    return;
                }
            }
            GameObject newProjectile = Instantiate(m_projectileToShoot, transform.position, Quaternion.identity,transform);
            newProjectile.transform.up = (m_direction.position - transform.position).normalized;
            projectilePool.Add(newProjectile);
            newProjectile.GetComponent<Projectile>().Speed = m_projectileSpeed;
        }        
    }

    public override void Activate()
    {
        m_active = true;
        
    }
    public override void Deactivate()
    {
        m_active = false;
    }

    public override void ActivateImmediately()
    {
        
    }
}