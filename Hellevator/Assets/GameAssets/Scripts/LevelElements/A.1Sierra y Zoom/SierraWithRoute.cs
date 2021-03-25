using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SierraWithRoute : MonoBehaviour
{
	[SerializeField] private List<Transform> m_puntosRuta;
	[SerializeField] private float m_speed = 10f;
	[SerializeField] private bool m_moving = false;
	private int m_currentIndex = 0;


	private void Update()
	{
		if (m_moving)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, m_puntosRuta[m_currentIndex].position, m_speed * Time.deltaTime);
			if(Vector3.Distance(this.transform.position, m_puntosRuta[m_currentIndex].position) <= 0.1f)
			{
				if (m_currentIndex < (m_puntosRuta.Count - 1))
				{
					m_currentIndex++;
				}
				else
				{
					this.enabled = false;
				}
			}
		}
	}

	public void StartMoving()
	{
		m_currentIndex = 0;
		m_moving = true;
	}
}
