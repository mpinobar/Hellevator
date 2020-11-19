using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float              m_chainExplosionDelay = 0.5f;
    [SerializeField] ParticleSystem     m_explosionParticles;
    [SerializeField] float              m_explosionRadius;
    [SerializeField] float              m_explosionForce;
    [SerializeField] LayerMask          m_explosionInteractionLayerMask;    
    BasicZombie                         m_demonCmp;

    private void Start()
    {
        m_demonCmp = GetComponent<BasicZombie>();
    }
    public void CreateExplosion()
    {
        //Debug.DrawLine(transform.position, transform.position + transform.up * m_explosionRadius, Color.red, 2f);
        m_demonCmp.RagdollLogicCollider.gameObject.SetActive(false);

        ExplosionVisuals();
        m_demonCmp.enabled = false;
        
        //PossessionManager.Instance.RemoveDemonPossession(transform);
        Collider2D [] colliders = Physics2D.OverlapCircleAll(transform.position,m_explosionRadius,m_explosionInteractionLayerMask);
        DemonBase demonInRange;
        DestructibleWall explodingWall;
        Boss boss;
        for (int i = 0; i < colliders.Length; i++)
        {
            demonInRange = colliders[i].GetComponentInParent<DemonBase>();
            explodingWall = colliders[i].GetComponentInParent<DestructibleWall>();
            boss = colliders[i].GetComponent<Boss>();
            if (demonInRange && demonInRange != GetComponentInParent<DemonBase>())
            {
                if (demonInRange.GetComponent<Explosion>())
                {
                    demonInRange.IsPossessionBlocked = true;
                    demonInRange.GetComponent<Explosion>().DelayExplosion(m_chainExplosionDelay);
                }

                if (!demonInRange.IsDead)
                {
                    demonInRange.Die(true);
                }
                
            }
            if (explodingWall)
            {
                explodingWall.Explode(transform.position, m_explosionForce);
            }
            if (boss)
            {
                boss.DamageBoss();
            }
        }

        m_demonCmp.UnparentBodyParts(m_explosionForce);       
        
    }

    public void DelayExplosion(float time)
    {
        Invoke(nameof(CreateExplosion), time);
    }

    public void ExplosionVisuals()
    {
        m_explosionParticles.Play();
        for (int i = 0; i < m_explosionParticles.transform.childCount; i++)
        {
            m_explosionParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }
    }

}
