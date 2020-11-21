﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    //public bool debug;
    List<SpikesWeightData> m_spikesData;
    [SerializeField] bool m_pushesRagdoll;

    private void Awake()
    {
        m_spikesData = new List<SpikesWeightData>();
    }

    // On trigger enter kill the character that collided. 
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

        if (cmpDemon != null)
        {
            bool isCounted = false;

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    isCounted = true;

                    //add the collider to the associated demon's collider list if it isnt already included
                    if (!m_spikesData[i].Colliders.Contains(collision))
                    {
                        m_spikesData[i].Colliders.Add(collision);
                    }
                }
            }
            if (!isCounted)
            {
                if (cmpDemon.IsControlledByPlayer)
                {
                    m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
                    cmpDemon.IsInDanger = true;
                    
                    cmpDemon.Die(true);
                }
                else
                {
                    if (!cmpDemon.IsInDanger)
                    {
                        m_spikesData.Add(new SpikesWeightData(cmpDemon, collision));
                        cmpDemon.IsInDanger = true;
                        cmpDemon.Die(true);
                    }
                }
            }

        }
    }

    private void LateUpdate()
    {
        //if (debug)
        //{
        //    Debug.LogError(m_spikesData.Count);
        //}
        for (int i = 0; i < m_spikesData.Count; i++)
        {
            m_spikesData[i].AssociatedDemon.IsInDanger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() != null && collision.gameObject.tag != "BodyCollider")
        {
            DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

            for (int i = 0; i < m_spikesData.Count; i++)
            {
                //if the demon is already inside the spikes
                if (cmpDemon == m_spikesData[i].AssociatedDemon)
                {
                    //remove the collider from the associated demon's collider list 
                    if (m_spikesData[i].Colliders.Contains(collision))
                    {
                        m_spikesData[i].Colliders.Remove(collision);

                        //all the limbs have exited the spikes
                        if (m_spikesData[i].Colliders.Count == 0)
                        {
                            m_spikesData.RemoveAt(i);
                            cmpDemon.IsInDanger = false;
                        }
                        else if (m_spikesData[i].Colliders.Count == 1 && m_spikesData[i].Colliders[0].tag == "BodyCollider")
                        {
                            m_spikesData.RemoveAt(i);
                            cmpDemon.IsInDanger = false;
                        }
                    }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_pushesRagdoll)
        {
            DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
            if (cmpDemon != null)
            {
                cmpDemon.Die(true);
                
                cmpDemon.ApplyForceToRagdoll((collision.transform.position - (Vector3) collision.contacts[0].point).normalized * 50f);
            }
        }
    }

}