using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] ParticleSystem     m_explosionParticles;
    [SerializeField] float              m_explosionRadius;
    [SerializeField] float              m_explosionForce;
    [SerializeField] LayerMask          m_explosionInteractionLayerMask;
    [SerializeField] List<GameObject>   m_limbsToUnparent;

    public void CreateExplosion()
    {
        Debug.DrawLine(transform.position, transform.position + transform.up * m_explosionRadius, Color.red, 2f);
        GetComponent<DemonBase>().RagdollLogicCollider.gameObject.SetActive(false);

        ExplosionVisuals();
        GetComponent<DemonBase>().enabled = false;
        
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
                demonInRange.Die(true);
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

        UnparentLimbs();
        
        
    }

    public void ExplosionVisuals()
    {
        m_explosionParticles.Play();
        for (int i = 0; i < m_explosionParticles.transform.childCount; i++)
        {
            m_explosionParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }
    }
    public void UnparentLimbs()
    {
        for (int i = 0; i < m_limbsToUnparent.Count; i++)
        {
            m_limbsToUnparent[i].transform.parent = null;
            m_limbsToUnparent[i].GetComponent<HingeJoint2D>().enabled = false;
            m_limbsToUnparent[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_limbsToUnparent[i].GetComponent<Rigidbody2D>().AddForce((Vector2.up + Random.Range(-2, 2) * Vector2.right) * m_explosionForce, ForceMode2D.Impulse);
        }
    }
}
