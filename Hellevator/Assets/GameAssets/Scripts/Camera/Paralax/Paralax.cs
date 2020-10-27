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



	float cameraDistMoved, previousCameraPosition, movedDistance = 0f;

	private bool m_paralaxIsSetUp = false;



	private bool countingTransition = false;
	private float distMoved = 0f;


	void Awake()
	{
		m_indexBackgroundInFront = 1;		

		//Variables necesarias para escalado automático con la ortographic size.
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
    void Update()
    {
		if (m_paralaxIsSetUp)
		{
			m_camera = Camera.main.transform;

			//Calculo de la nueva escala del fondo segun el ortographic size de la cámara 
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


			//Movimiento del fondo. 

			// Calcular la diferencia entre la posicion en X de la camara entre este frame y el anterior
			cameraDistMoved = m_camera.transform.position.x - previousCameraPosition;
			previousCameraPosition = m_camera.transform.position.x;

			this.transform.position = new Vector3(this.transform.position.x + cameraDistMoved * m_parallaxSpeed, m_camera.position.y, this.transform.position.z);

			//this.transform.Translate(new Vector3(cameraDistMoved * m_parallaxSpeed, 0, 0));
			//m_backgrounds[m_indexBackgroundInFront].localPosition = new Vector3(indexParalax * m_length, 0, 0);


			movedDistance = movedDistance + cameraDistMoved * (1 - m_parallaxSpeed);


			if (Input.GetKeyDown(KeyCode.L))
			{
				//print(this.gameObject.name + " movedDistance: " + movedDistance);
				print(m_camera.transform.position.x);
				countingTransition = !countingTransition;
				if (!countingTransition)
				{
					//print(this.gameObject.name + " distancia de la transicion: " + distMoved);
				}
			}

			if (countingTransition)
			{
				distMoved = distMoved + cameraDistMoved;
			}


			//Coloca los fondos a length de distancia el uno del otro. (Derecha)
			float halfNumberOfBackgrounds = m_backgrounds.Length / 2;

			for (int i = 1; i < halfNumberOfBackgrounds + 1; i++)
			{
				int index = m_indexBackgroundInFront - i;

				if (index < 0)
				{
					index = m_backgrounds.Length + index;
					float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x - i * m_length;
					//float newXPos = indexParalax * m_length - i * m_length;
					m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
				}
				else
				{
					float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x - i * m_length;
					//float newXPos = indexParalax * m_length - i * m_length;
					m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
				}
			}


			//Coloca los fondos a length de distancia el uno del otro. (Izquierda)

			for (int i = 1; i < halfNumberOfBackgrounds + 1; i++)
			{
				int index = m_indexBackgroundInFront + i;

				if (index == m_backgrounds.Length)
				{
					index = 0;
					float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
					//float newXPos = indexParalax * m_length + i * m_length;
					m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
				}
				else if (index > m_backgrounds.Length)
				{
					index = index - m_backgrounds.Length;
					float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
					//float newXPos = indexParalax * m_length + i * m_length;
					m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
				}
				else
				{
					float newXPos = m_backgrounds[m_indexBackgroundInFront].localPosition.x + i * m_length;
					//float newXPos = indexParalax * m_length + i * m_length;
					m_backgrounds[index].localPosition = new Vector3(newXPos, 0, 0);
				}
			}


			
			// Comprueba si necesita que los fondos cambien del sitio para que no se quede un hueco sin fondo. 
			if (movedDistance > m_length)
			{
				MoveBackgroundToRight();
			}
			else if (movedDistance < (m_length * -1))
			{
				MoveBackgroundToLeft();
			}
		}
	}


	void MoveBackgroundToLeft()
	{
		m_startingPosX -= m_length;
		movedDistance += m_length ;

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
	void MoveBackgroundToRight()
	{
		m_startingPosX += m_length;
		movedDistance -= m_length;
		indexParalax++;
		int i = m_indexBackgroundInFront - 1;
		if (i == -1)
		{
			i = m_backgrounds.Length - 1;
		}

		m_backgrounds[i].position = new Vector3(m_backgrounds[i].position.x + m_length * m_backgrounds.Length, m_backgrounds[i].position.y, m_backgrounds[i].position.z);
		m_indexBackgroundInFront = m_indexBackgroundInFront + 1;
		if (m_indexBackgroundInFront == m_backgrounds.Length)
		{
			m_indexBackgroundInFront = 0;
		}
	}


	public void SetUpParalax()
	{
		if (!m_paralaxIsSetUp)
		{
			float posX = this.transform.position.x;
			this.transform.position = new Vector3(Camera.main.transform.position.x, this.transform.position.y, this.transform.position.z);

			m_startingPosX = Camera.main.transform.position.x;
			pruebas = m_startingPosX;
			previousCameraPosition = m_startingPosX;

			m_paralaxIsSetUp = true;

			//Make it so that the backgrounds are in the right position
			
			float diff = m_startingPosX - posX;
			

			float correctionsNeeded = diff / this.GetComponentInChildren<SpriteRenderer>().bounds.size.x;
			//if(diff < 0)
			//{
			//	for(int i = 0; i < (int)correctionsNeeded; i++)
			//	{
			//		MoveBackgroundToLeft();
			//	}
			//}
			//else if (diff > 0)
			//{
			//	for (int i = 0; i < (int)correctionsNeeded; i++)
			//	{
			//		MoveBackgroundToLeft();
			//	}
			//}
		}
	}
}
