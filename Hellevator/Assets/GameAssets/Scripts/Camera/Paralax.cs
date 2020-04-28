using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
	private float m_length = 0f;
	private float m_startingPosX = 0f;
	[Tooltip("Cuanto más cerca de 1 menos paralax habra, si es 1 no habrá")]
	[Range(0,1)]
	[SerializeField]private float m_parallaxSpeed = 0f;

	private Transform m_camera = null;

	
    void Awake()
    {
		m_camera = Camera.main.transform;
		m_startingPosX = this.transform.position.x;
		m_length = this.GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		float temp = (m_camera.position.x * (1 - m_parallaxSpeed));
		float dist = (m_camera.position.x * m_parallaxSpeed);

		transform.position = new Vector3(m_startingPosX + dist, transform.position.y, transform.position.z);

		if(temp > m_startingPosX + m_length)
		{
			m_startingPosX = m_startingPosX + m_length;
		}
		else if(temp < m_startingPosX - m_length)
		{
			m_startingPosX = m_startingPosX + m_length;
		}
	}
}
