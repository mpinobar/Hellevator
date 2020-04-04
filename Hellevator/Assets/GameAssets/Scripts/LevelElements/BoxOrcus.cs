using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOrcus : MonoBehaviour
{
	[SerializeField] private float m_distanceMax = 0f;
	private Transform m_orcus = null;
	private Rigidbody2D m_cmpRB = null;
    Collider2D[] m_parts;

	private void Awake()
	{
		m_cmpRB = this.GetComponent<Rigidbody2D>();
        m_parts = ReturnComponentsInChildren<Collider2D>();
        for (int i = 0; i < m_parts.Length; i++)
        {
            m_parts[i].enabled = false;
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



    private void Update()
	{
		if (m_cmpRB.isKinematic)
		{
			if (Vector3.Distance(this.transform.position, m_orcus.position) >= m_distanceMax)
			{
				m_cmpRB.isKinematic = false;
			}
		}
	}


	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (collision.collider.GetComponentInParent<DemonBase>() != null)
		{
			if (collision.collider.GetComponentInParent<Orcus>() != null)
			{
                if (collision.collider.GetComponentInParent<Orcus>().UsedSkill)
                {
                    GetComponent<Collider2D>().enabled = false;
                    for (int i = 0; i < m_parts.Length; i++)
                    {
                        m_parts[i].enabled = true;
                        m_parts[i].GetComponent<Rigidbody2D>().isKinematic = false;
                    }
                }
                else
                {
                    m_orcus = collision.collider.transform;
                    m_cmpRB.isKinematic = false;
                }
			}
			else
			{
				m_cmpRB.isKinematic = true;
				m_cmpRB.velocity = Vector3.zero;
			}

			m_orcus = collision.collider.transform.parent.transform;
		}
	}
}
