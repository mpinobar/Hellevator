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
    bool                                m_hasExploded = false;
	[SerializeField] AudioClip m_explosionClip = null;

	private void Start()
    {
        m_demonCmp = GetComponent<BasicZombie>();
    }
    public void CreateExplosion()
    {
        
        if (m_hasExploded)
            return;
        Debug.LogError("Exploding");
        //Debug.DrawLine(transform.position, transform.position + transform.up * m_explosionRadius, Color.red, 2f);
        //m_demonCmp.RagdollLogicCollider.gameObject.SetActive(false);
        AudioManager.Instance.PlayAudioSFX(m_explosionClip, false, 2f);
        CameraManager.Instance.CameraShakeMedium();
        m_demonCmp.IsPossessionBlocked = true;
        ExplosionVisuals();
        m_demonCmp.enabled = false;
        m_hasExploded = true;
        //PossessionManager.Instance.RemoveDemonPossession(transform);
        Collider2D [] colliders = Physics2D.OverlapCircleAll(m_demonCmp.Torso.position,m_explosionRadius,m_explosionInteractionLayerMask);
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
                if (demonInRange.GetComponent<Petrification>())
                {
                    demonInRange.IsPossessionBlocked = true;
                    demonInRange.GetComponent<Petrification>().Petrify();
                }

                if (!demonInRange.IsDead)
                {
                    demonInRange.Die(true);
                }
                
            }
            if (explodingWall)
            {
                explodingWall.Explode(m_demonCmp.Torso.position, m_explosionForce);
            }
            if (boss)
            {
                boss.DamageBoss();
            }
        }

        m_demonCmp.UnparentBodyParts(m_explosionForce);
        enabled = false;
        
    }

    public void DelayExplosion(float time)
    {
        if (m_hasExploded)
            return;
        //m_hasExploded = true;
        Invoke(nameof(CreateExplosion), time);
    }

    public void ExplosionVisuals()
    {
        m_explosionParticles.transform.parent = null;
        m_explosionParticles.transform.position = m_demonCmp.Torso.position;
        m_explosionParticles.Play();
        for (int i = 0; i < m_explosionParticles.transform.childCount; i++)
        {
            m_explosionParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }
    }
    public void SetCantExplode()
    {
        m_hasExploded = true;
    }
}
