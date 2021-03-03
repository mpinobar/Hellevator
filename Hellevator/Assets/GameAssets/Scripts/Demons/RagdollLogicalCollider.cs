using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollLogicalCollider : MonoBehaviour
{

    DemonBase m_parentDemon;

    public DemonBase ParentDemon { get => m_parentDemon; set => m_parentDemon = value; }

    bool m_inParts;

    List<Spikes> m_triggeringSpikes;

    private void Start()
    {
        m_triggeringSpikes = new List<Spikes>();
        m_parentDemon = GetComponentInParent<DemonBase>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 0 << 0 || collision.gameObject.layer == 1 << 8)
        {
            if (m_parentDemon)
            {
                //m_parentDemon.Torso
                //Debug.LogError("Resetting ragdoll velocity of " + m_parentDemon.name);
                m_parentDemon.ResetRagdollVelocity();
            }
            //for (int i = 0; i < LimbsColliders.Length; i++)
            //{
            //    LimbsColliders[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            //}


            if (m_parentDemon.transform.parent != null)
            {
                SpawnerMatadero sm = GetComponentInParent<SpawnerMatadero>();
                if (sm != null)
                {
                    sm.DetachCharacter(m_parentDemon);
                }
            }
            if (!m_parentDemon.enabled)
            {
                SpawnerMatadero sm = GetComponentInParent<SpawnerMatadero>();
                if (sm != null)
                {
                    sm.DetachPart(transform.parent);
                    m_inParts = true;
                    m_parentDemon.enabled = true;
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_inParts)
        {
            m_parentDemon.IsInDanger = true;
            m_parentDemon.IsPossessionBlocked = true;
        }
        if(m_triggeringSpikes.Count > 0)
            m_parentDemon.IsInDanger = true;        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Spikes spikes))
        {
            m_triggeringSpikes.Add(spikes);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Spikes spikes))
        {
            if (m_triggeringSpikes.Contains(spikes))
                m_triggeringSpikes.Remove(spikes);
            if(m_triggeringSpikes.Count == 0)
            {
                m_parentDemon.IsInDanger = false;
            }
        }
    }
}
