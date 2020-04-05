using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabbedTorso : MonoBehaviour
{
	[SerializeField]private float m_speed = 0f;
	[SerializeField] private float m_minDistance = 0f;
	private bool m_isGrabbed = false;
	private Rigidbody2D m_rb = null;
	private Transform m_linkedTransform = null;

	public bool IsGrabbed { get => m_isGrabbed; set => m_isGrabbed = value; }
	public Transform LinkedTransform { get => m_linkedTransform; set => m_linkedTransform = value; }


	private void Awake()
	{
		m_rb = this.GetComponent<Rigidbody2D>();
	}

	// Update is called once per frame
	void Update()
    {
		if (m_isGrabbed)
		{
			float m_modifier = m_linkedTransform.position.x - this.transform.position.x;
			print(m_modifier);
			//m_rb.velocity = new Vector2(m_modifier * m_speed * Time.deltaTime, m_rb.velocity.y);

			m_rb.AddForce(new Vector2(m_modifier * m_speed * Time.deltaTime, 0), ForceMode2D.Force);

			//if (Vector2.Distance(this.transform.position, m_linkedTransform.position) > m_minDistance)
			//{	
				
			//}

		}
    }
}
