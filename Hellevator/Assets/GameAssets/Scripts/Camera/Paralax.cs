using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paralax : MonoBehaviour
{
	private float m_length, m_height = 0f;
	private float m_lengthOG, m_heightOG = 0f;
	private float m_ratioX, m_ratioY = 0f;
	private float m_startingPosX = 0f;
	[Tooltip("Cuanto más cerca de 1 menos paralax habra, si es 1 no habrá")]
	[Range(0,1)]
	[SerializeField] private float m_parallaxSpeed = 0f;
	[SerializeField] private Transform[] m_backgrounds = new Transform[3];
	private int m_indexBackgroundInFront = 0;

	private SpriteRenderer m_cmpSR = null;

	private Transform m_camera = null;
	private Camera m_cmpCamera = null;

	float pruebas = 0f;

	float worldScreenHeight = 0f;
	float worldScreenWidth = 0f;

	int indexParalax = 0;

	void Awake()
	{
		m_indexBackgroundInFront = 1;
		m_startingPosX = this.transform.position.x;
		pruebas = m_startingPosX;

		m_camera = Camera.main.transform;
		m_cmpCamera = Camera.main;

		m_cmpSR = m_backgrounds[0].GetComponent<SpriteRenderer>();

		m_heightOG = m_cmpSR.bounds.size.y;
		m_lengthOG = m_cmpSR.bounds.size.x;


		worldScreenHeight = m_cmpCamera.orthographicSize * 2.0f;
		worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;


		m_ratioX = m_lengthOG / worldScreenWidth;
		m_ratioY = m_heightOG / worldScreenHeight;
	}

    // Update is called once per frame
    void FixedUpdate()
    {
		m_length = this.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
		m_height = this.GetComponentInChildren<SpriteRenderer>().bounds.size.y;

		worldScreenHeight = m_cmpCamera.orthographicSize * 2.0f;
		worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

		for (int i = 0; i < m_backgrounds.Length; i++)
		{
			Vector3 localScale = m_backgrounds[i].transform.localScale;

			localScale.y = m_ratioY * worldScreenHeight / m_heightOG;
			localScale.x = m_ratioX * worldScreenWidth / m_lengthOG;

			m_backgrounds[i].transform.localScale = localScale;
		}

		float temp = (m_camera.position.x * (1 - m_parallaxSpeed));
		float dist = (m_camera.position.x * m_parallaxSpeed);
		this.transform.position = new Vector3(m_startingPosX + dist, transform.position.y, transform.position.z);
		m_backgrounds[m_indexBackgroundInFront].localPosition = new Vector3(indexParalax * m_length, 0, 0);
		print(m_backgrounds[m_indexBackgroundInFront].localPosition);

		float halfNumberOfBackgrounds = m_backgrounds.Length / 2;

		for (int i = 1; i < halfNumberOfBackgrounds + 1; i++)
		{
			int index = m_indexBackgroundInFront - i;

			if (index < 0)
			{
				index = m_backgrounds.Length + index;
				float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x - i * m_length;
				m_backgrounds[index].localPosition = new Vector3(newXPos,0, 0);
			}
			else
			{
				float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x - i * m_length;
				m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
			}
		}

		for (int i = 1; i < halfNumberOfBackgrounds + 1; i++)
		{
			int index = m_indexBackgroundInFront + i;

			if (index == m_backgrounds.Length)
			{
				index = 0;
				float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
				m_backgrounds[index].localPosition = new Vector3(newXPos, 0,0);
			}
			else if (index > m_backgrounds.Length)
			{
				index = index - m_backgrounds.Length;
				float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
				m_backgrounds[index].localPosition = new Vector3(newXPos, 0,0);
			}
			else
			{
				float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
				m_backgrounds[index].localPosition = new Vector3(newXPos, 0,0);
			}
		}


		if (temp > pruebas + m_length)
		{
			pruebas += m_length;
			indexParalax++;
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
			indexParalax--;
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
