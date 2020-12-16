using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : ActivatedBase
{

    [SerializeField] GameObject m_projectileToShoot;
    [SerializeField] Transform  m_direction;
    [SerializeField] float      m_initialWaitTimeBeforeShooting = 0f;
    [SerializeField] float      m_timeIntervalBetweenShots = 2f;
    float                       m_timeIntervalBetweenShotsTimer = 0;
    [SerializeField] float      m_waitFromAnimationStart = 2f;
    [SerializeField] float      m_projectileSpeed = 4f;
    [SerializeField] bool       m_activatedWithButton;
    [ColorUsage(true,true)]
    [SerializeField] Color      m_spriteColor;
    SpriteRenderer              m_spriteCmp;

    int m_maxNumberOfProjectiles = 5;
    List<GameObject> m_projectilePool;

    bool m_active;
    Animator m_anim;


    // Start is called before the first frame update
    void Start()
    {
        m_spriteCmp = GetComponentInChildren<SpriteRenderer>();
        if (m_spriteCmp)
            m_spriteCmp.color = m_spriteColor;
        m_anim = GetComponentInParent<Animator>();
        m_projectilePool = new List<GameObject>();
        for (int i = 0; i < m_maxNumberOfProjectiles; i++)
        {
            m_projectilePool.Add(Instantiate(m_projectileToShoot, transform.position, Quaternion.identity, transform));
            m_projectilePool[i].transform.up = (m_direction.position - transform.position).normalized;
            m_projectilePool[i].SetActive(false);
        }
        if (!m_activatedWithButton)
        {
            Activate();
        }
        //InvokeRepeating(nameof(ShootProjectile), m_initialWaitTimeBeforeShooting, m_timeIntervalBetweenShots);
    }

    private void ShootProjectile()
    {
        if (m_active)
        {            
            for (int i = 0; i < m_projectilePool.Count; i++)
            {
                if (!m_projectilePool[i].activeSelf)
                {
                    m_projectilePool[i].transform.position = transform.position;
                    m_projectilePool[i].SetActive(true);
                    m_projectilePool[i].GetComponent<Projectile>().Speed = m_projectileSpeed;
                    return;
                }
            }
            GameObject newProjectile = Instantiate(m_projectileToShoot, transform.position, Quaternion.identity,transform);
            newProjectile.transform.up = (m_direction.position - transform.position).normalized;
            m_projectilePool.Add(newProjectile);
            newProjectile.GetComponent<Projectile>().Speed = m_projectileSpeed;
        }
    }

    private void Update()
    {
        if(m_initialWaitTimeBeforeShooting > 0)
        {
            m_initialWaitTimeBeforeShooting -= Time.deltaTime;
        }
        else if(m_active)
        {
            m_timeIntervalBetweenShotsTimer -= Time.deltaTime;
            if(m_timeIntervalBetweenShotsTimer <= 0)
            {
                m_timeIntervalBetweenShotsTimer = m_timeIntervalBetweenShots;
                if (m_anim)
                {
                    m_anim.SetTrigger("Attack");
                    StartCoroutine(DelayProjectile(m_waitFromAnimationStart));
                }
                else
                {
                    ShootProjectile();
                }
            }
        }
    }

    IEnumerator DelayProjectile (float time)
    {
        yield return new WaitForSeconds(time);
        ShootProjectile();
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
