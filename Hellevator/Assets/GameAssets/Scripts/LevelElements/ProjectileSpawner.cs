using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : ButtonActivatedBase
{

    [SerializeField] GameObject m_projectileToShoot;
    [SerializeField] float m_initialWaitTimeBeforeShooting = 0f;
    [SerializeField] float m_timeIntervalBetweenShots = 2f;
    [SerializeField] float m_projectileSpeed = 4f;
    [SerializeField] bool m_activatedWithButton;

    int maxNumberOfProjectiles = 5;
    List<GameObject> projectilePool;

    // Start is called before the first frame update
    void Start()
    {
        projectilePool = new List<GameObject>();
        for (int i = 0; i < maxNumberOfProjectiles; i++)
        {
            projectilePool.Add(Instantiate(m_projectileToShoot, transform.position, transform.rotation, transform));
            projectilePool[i].SetActive(false);
        }
        if (!m_activatedWithButton)
        {
            Activate();
        }
    }

    private void ShootProjectile()
    {
        for (int i = 0; i < projectilePool.Count; i++)
        {
            if (!projectilePool[i].activeSelf)
            {
                projectilePool[i].SetActive(true);
                projectilePool[i].GetComponent<Projectile>().Speed = m_projectileSpeed;
                return;
            }
        }
        GameObject newProjectile = Instantiate(m_projectileToShoot, transform.position, transform.rotation,transform);
        projectilePool.Add(newProjectile);
        newProjectile.GetComponent<Projectile>().Speed = m_projectileSpeed;
    }

    public override void Activate()
    {
        InvokeRepeating(nameof(ShootProjectile), m_initialWaitTimeBeforeShooting, m_timeIntervalBetweenShots);
    }
}
