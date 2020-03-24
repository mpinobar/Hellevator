using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orcus : DemonBase
{
	#region Variables

	[Space]

	[SerializeField] private float m_IAMaxSpeed = 0f;
	private float m_IAcurrentSpeed = 0f;
	[SerializeField] private float m_IAcceleration = 0f;
	[SerializeField] private float m_IAStoppingDistance = 0f;

	[SerializeField] private List<Transform> m_IAPatrolPoints = new List<Transform> (0);
	private int m_IACurrentPatrolPoint = 0;

	private bool m_IAStopping = false;
	private float m_IADirectionSpeedModifier = 1;

	private Rigidbody2D m_IACmpRb = null;
	private Vector3 IAVelocity = Vector3.zero;

	#endregion Variables


	public override void Jump()
	{
		throw new System.NotImplementedException();
	}

	public override void Move(float xInput)
	{
		throw new System.NotImplementedException();
	}

	public override void ToggleWalkingParticles(bool active)
	{
		throw new System.NotImplementedException();
	}

	public override void UseSkill()
	{
		throw new System.NotImplementedException();
	}

	private void Awake()
	{
		m_IACmpRb = this.GetComponent<Rigidbody2D>();
		m_IACurrentPatrolPoint = 0;
		float xPatrolCoord = m_IAPatrolPoints[m_IACurrentPatrolPoint].position.x;
		if ((xPatrolCoord - this.transform.position.x) < 0)
		{
			m_IADirectionSpeedModifier = -1;
		}
		else
		{
			m_IADirectionSpeedModifier = 1;
		}
	}

	// Start is called before the first frame update
	void Start()
    {
        
    }

	// Update is called once per frame
	protected override void Update()
    {
		base.Update();
		IAUpdate();
    }

	void IAUpdate()
	{
		if (!m_IAStopping)
		{
			float xPatrolCoord = m_IAPatrolPoints[m_IACurrentPatrolPoint].position.x;
			float distance = Mathf.Abs(xPatrolCoord - this.transform.position.x);

			if (distance <= m_IAStoppingDistance)
			{
				m_IAStopping = true;
				m_IADirectionSpeedModifier = m_IADirectionSpeedModifier * (-1);
			}
		}

		IAVelocity = m_IACmpRb.velocity;

		m_IAcurrentSpeed = m_IAcurrentSpeed + m_IAcceleration * Time.deltaTime * m_IADirectionSpeedModifier;

		if(m_IAcurrentSpeed * m_IADirectionSpeedModifier > m_IAMaxSpeed)
		{
			m_IAcurrentSpeed = m_IAMaxSpeed * m_IADirectionSpeedModifier;
		}
		
		if (m_IAStopping)
		{
			if(m_IAcurrentSpeed * m_IADirectionSpeedModifier <= 0)
			{
				IASetNextPatrolPoint();
			}
		}

		m_IACmpRb.velocity = new Vector3(m_IAcurrentSpeed, IAVelocity.y, 0);

		print(m_IAcurrentSpeed);
	}

	void IASetNextPatrolPoint()
	{
		m_IAStopping = false;
		
		if (m_IACurrentPatrolPoint == (m_IAPatrolPoints.Count - 1))
		{
			m_IACurrentPatrolPoint = 0;
		}
		else
		{
			m_IACurrentPatrolPoint = m_IACurrentPatrolPoint + 1;
		}
	}
}
