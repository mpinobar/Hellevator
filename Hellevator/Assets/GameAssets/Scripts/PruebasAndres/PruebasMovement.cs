using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PruebasMovement : TemporalSingleton<PruebasMovement>
{

	[SerializeField] private CinemachineVirtualCamera m_currentCamera = null;

	[SerializeField] private GameObject m_otherGO = null;
	private GameObject m_startingGO = null;

	private GameObject m_currentGO = null;
 

	[SerializeField] private float m_speed = 0f;
	[SerializeField] private float m_jumpStrength = 0f;

	private DemonBase m_currentEnemy = null;

    // Start is called before the first frame update
    void Start()
    {
		m_startingGO = this.gameObject;
		m_currentGO = m_startingGO;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	public void ChangeCamera()
	{
		if(m_currentGO == m_startingGO)
		{
			m_currentGO = m_otherGO;
		}
		else
		{
			m_currentGO = m_startingGO;
		}

		m_currentCamera.Follow = m_currentGO.transform;
		m_currentCamera.LookAt = m_currentGO.transform;
	}

	public void MoveSquare(float x)
	{
		this.transform.position = new Vector3(this.transform.position.x + x * Time.deltaTime * m_speed, this.transform.position.y, this.transform.position.z);
	}

	public void Jump()
	{
		this.GetComponent<Rigidbody2D>().AddForce(transform.up * m_jumpStrength);
	}
}
