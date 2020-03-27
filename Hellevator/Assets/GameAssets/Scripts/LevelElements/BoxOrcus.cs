using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxOrcus : MonoBehaviour
{
	[SerializeField] private float m_distanceMax = 0f;
	private Transform m_orcus = null;
	private Rigidbody2D m_cmpRB = null;

	private void Awake()
	{
		m_cmpRB = this.GetComponent<Rigidbody2D>();
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
				m_orcus = collision.collider.transform;
				m_cmpRB.isKinematic = false;
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
