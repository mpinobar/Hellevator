using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
	private float m_length = 0f;
	private float m_startingPosX = 0f;
	[Tooltip("Cuanto más cerca de 1 menos paralax habra, si es 1 no habrá")]
	[Range(0,1)]
	[SerializeField] private float m_parallaxSpeed = 0f;
	[SerializeField] private Transform[] m_backgrounds = new Transform[3];
	private int m_indexBackgroundInFront = 0;

	private Transform m_camera = null;

	float pruebas = 0f;

	
    void Awake()
    {
		m_indexBackgroundInFront = 1;
		m_camera = Camera.main.transform;
		m_startingPosX = this.transform.position.x;
		pruebas = m_startingPosX;
		m_length = this.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		float temp = (m_camera.position.x * (1 - m_parallaxSpeed));

		float dist = (m_camera.position.x * m_parallaxSpeed);

		transform.position = new Vector3(m_startingPosX + dist, transform.position.y, transform.position.z);

		if(temp > pruebas + m_length)
		{
			pruebas += m_length;
			int i = m_indexBackgroundInFront - 1;
			if(i == -1)
			{
				i = m_backgrounds.Length - 1;
			}

			m_backgrounds[i].position = new Vector3(m_backgrounds[i].position.x + m_length * m_backgrounds.Length, m_backgrounds[i].position.y, m_backgrounds[i].position.z);
			m_indexBackgroundInFront = m_indexBackgroundInFront + 1;
			if(m_indexBackgroundInFront == m_backgrounds.Length)
			{
				m_indexBackgroundInFront = 0;
			}
		}
		else if(temp < pruebas - m_length)
		{
			pruebas -= m_length;
			int i = m_indexBackgroundInFront + 1;
			if (i == m_backgrounds.Length)
			{
				i = 0;
			}
			m_backgrounds[i].position = new Vector3(m_backgrounds[i].position.x - m_length * m_backgrounds.Length, m_backgrounds[i].position.y, m_backgrounds[i].position.z);
			m_indexBackgroundInFront = m_indexBackgroundInFront - 1;
			if (m_indexBackgroundInFront == -1)
			{
				m_indexBackgroundInFront = m_backgrounds.Length - 1;
			}
		}
	}
}
