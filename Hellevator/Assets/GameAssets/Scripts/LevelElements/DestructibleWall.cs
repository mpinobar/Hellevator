using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleWall : ActivatedBase
{    
    Collider2D[] m_parts;
    [SerializeField] float m_timeToDestroyPartAfterExplosion = 4f;

    private void Awake()
    {
		m_parts = ReturnComponentsInChildren<Collider2D>();
        for (int i = 0; i < m_parts.Length; i++)
        {
            if (!m_parts[i].GetComponent<Spikes>())
            {
                m_parts[i].enabled = false;
            }
            m_parts[i].GetComponent<Rigidbody2D>().isKinematic = true;
        }
    }

    /// <summary>
    /// Returns all child component references of specified component, excluding the parent
    /// </summary>
    /// <typeparam name="T">The specified component to look for</typeparam>
    /// <returns>An array with the components</returns>
    private T[] ReturnComponentsInChildren<T>()
    {
        T[] array = GetComponentsInChildren<T>();
        T[] returnedArray = new T[array.Length - 1];

        for (int i = 0; i < returnedArray.Length; i++)
        {
            returnedArray[i] = array[i + 1];
        }
        return returnedArray;
    }

    public void Explode(Vector3 origin, float force)
    {
        for (int i = 0; i < m_parts.Length; i++)
        {
            if (m_parts[i].GetComponent<Spikes>())
            {
                m_parts[i].enabled = false;
            }
            else
            {
                m_parts[i].enabled = true;
            }

            m_parts[i].transform.parent = null;
            m_parts[i].GetComponent<Rigidbody2D>().isKinematic = false;
            Dissolve dis = m_parts[i].GetComponent<Dissolve>();
            dis?.StartDissolve();
            //m_parts[i].GetComponent<Rigidbody2D>().velocity = (m_parts[i].transform.position - origin).normalized * force;
            m_parts[i].GetComponent<Rigidbody2D>().AddForce((m_parts[i].transform.position - origin).normalized * force, ForceMode2D.Impulse);
            Destroy(m_parts[i].gameObject, m_timeToDestroyPartAfterExplosion);
        }
		
        Activate();
        Destroy(gameObject);
    }
}
