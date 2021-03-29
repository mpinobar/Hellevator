using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalTransport : MonoBehaviour
{
    [SerializeField] public float m_speed = 2f;
    [SerializeField] private TransportDirection direction = TransportDirection.Right;
    public List<DemonBase> m_enemiesOnPreassurePlate;
    [SerializeField] private LayerMask m_enemyLayerMask;

    public float dir;

    private void Awake()
    {
        m_enemiesOnPreassurePlate = new List<DemonBase>();

        dir = (int)direction * 2 - 1;

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_enemiesOnPreassurePlate.Count; i++)
        {
            if (m_enemiesOnPreassurePlate[i] != null)
            {
                m_enemiesOnPreassurePlate[i].DragMovement(dir * m_speed);
            }
            else
            {
                m_enemiesOnPreassurePlate.TrimExcess();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;
        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();

        if (cmpDemon != null)
        {
            cmpDemon.DragMovement(m_speed * dir);
            m_enemiesOnPreassurePlate.Add(cmpDemon);

        }
        else
        {
            RagdollLogicalCollider ragdollCollider = collision.transform.GetComponent<RagdollLogicalCollider>();
            if (ragdollCollider)
            {
                cmpDemon = ragdollCollider.ParentDemon;
                if (!m_enemiesOnPreassurePlate.Contains(cmpDemon))
                {
                    //Debug.LogError("Added demon " + cmpDemon.name + " collider is " + collision.name);
                    m_enemiesOnPreassurePlate.Add(cmpDemon);
                    cmpDemon.DragMovement(m_speed * dir);
                    //m_currentWeight += cmpDemon.Weight;
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
            return;

        DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
        if (cmpDemon != null )
        {

            if (m_enemiesOnPreassurePlate.Contains(cmpDemon))
            {
                StopAllCoroutines();
                cmpDemon.DragMovement(0);
                StartCoroutine(PushedBodyInertia(cmpDemon));
                //m_spikesData.RemoveAt(i);
                m_enemiesOnPreassurePlate.Remove(cmpDemon);
            }
           
        }
        else
        {
            RagdollLogicalCollider ragdollCollider = collision.transform.GetComponent<RagdollLogicalCollider>();
            if (ragdollCollider)
            {
                cmpDemon = ragdollCollider.ParentDemon;
                if (m_enemiesOnPreassurePlate.Contains(cmpDemon))
                {
                    //Debug.LogError("Added demon " + cmpDemon.name + " collider is " + collision.name);
                    StopAllCoroutines();
                    cmpDemon.DragMovement(0);
                    StartCoroutine(PushedBodyInertia(cmpDemon));
                    //m_spikesData.RemoveAt(i);
                    m_enemiesOnPreassurePlate.Remove(cmpDemon);
                    //m_currentWeight += cmpDemon.Weight;
                }
            }
        }
    }

    IEnumerator PushedBodyInertia(DemonBase demon)
    {
        float maxTimeDrag = 0.25f;
        float time = maxTimeDrag;
        while (time > 0)
        {
            if (demon && demon.IsGrounded())
            {
                demon.DragMovement(m_speed * dir);
            }
            else
            {
                time = 0;
            }
            time -= Time.deltaTime;
            yield return null;
        }
    }
}
