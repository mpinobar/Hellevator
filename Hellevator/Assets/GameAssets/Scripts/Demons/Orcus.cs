using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orcus : DemonBase
{
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
		IAAwake();
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

	private Vector3 GetVectorFromAngle(float angle)
	{
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	private float GetAngleFromVector(Vector3 dir)
	{

		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		if (n < 0) n += 360;

		return n;
	}

	#region IA


	#region Variables

	[Space]

	[SerializeField] private float m_IAMaxSpeed = 0f;
	private float m_IAcurrentSpeed = 0f;
	[SerializeField] private float m_IAcceleration = 0f;
	[SerializeField] private float m_IAStoppingDistance = 0f;
	[SerializeField] private EnemyState m_IACurrentState = EnemyState.None;

	[Space]

	[SerializeField] private float m_IADetectionRange = 0f;
	[SerializeField] private float m_IADetectionAngle = 0f;
	[SerializeField] private float m_IADetectionRayCount = 0f;
	[SerializeField] private LayerMask m_IADetectionLayers;
	[SerializeField] private LayerMask m_IADetectionLayersForForwardVector;

	[Space]
	[SerializeField] private List<Transform> m_IAPatrolPoints = new List<Transform>(0);
	private int m_IACurrentPatrolPoint = 0;

	[Space]
	[SerializeField] private float m_maxDistanceChasingFromSpawn = 0f;
	[SerializeField] private float m_maxDistanceChasingFromPlayerX = 0f;
	[SerializeField] private float m_maxDistanceChasingFromPlayerY = 0f;
	[SerializeField] private float m_maxDistanceChasingFromPlayer = 0f;

	private Transform m_IACharacterBeingChased = null;


	private bool m_IAStopping = false;
	private float m_IADirectionSpeedModifier = 1;

	private Rigidbody2D m_IACmpRb = null;
	private Vector3 m_IAVelocity = Vector3.zero;
	
	private Vector3 m_IAStartingPos = Vector3.zero;

	#endregion Variables

	void IAAwake()
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

		m_IAStartingPos = this.transform.position;
	}

	void IAUpdate()
	{

		if (Input.GetKeyDown(KeyCode.M))
		{
			print(IASenseForPlayer());
		}

		switch (m_IACurrentState)
		{
			case EnemyState.Chasing:
				{

					IAChaseUpdate();

					if (!IACheckStopChaseConditions())
					{

					}
					else
					{
						IAGoBackToPatrol();
					}
				}
				break;
			case EnemyState.Patrol:
				{
					if (!IASenseForPlayer())
					{
						IAPatrolUpdate();
					}
					else
					{
						IAStartChase();
					}
				}
				break;
			case EnemyState.GoingBack:
				break;
			case EnemyState.None:
				break;
			default:
				break;
		}
	}


	bool IASenseForPlayer()
	{
		float angleIncrease = m_IADetectionAngle / m_IADetectionRayCount;

		RaycastHit2D rayNormal = Physics2D.Raycast(this.transform.position, -this.transform.up, 2, m_IADetectionLayersForForwardVector);
		
		Vector2 forwardVector = Vector3.zero;

		float angle = 0f;

		if (m_IADirectionSpeedModifier > 0)
		{
			if (GetAngleFromVector(rayNormal.normal) >= 90)
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = Vector2.Angle(forwardVector, this.transform.right);
				
				angle = angle - m_IADetectionAngle / 2f;
			}
			else
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = -Vector2.Angle(forwardVector, this.transform.right);
				
				angle = angle - m_IADetectionAngle / 2f;
			}
		}
		else
		{

			if (GetAngleFromVector(rayNormal.normal) >= 90)
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = Vector2.Angle(forwardVector, this.transform.right);
				
				angle = 180 + angle + m_IADetectionAngle / 2f;
			}
			else
			{
				forwardVector = Vector2.Perpendicular(rayNormal.normal);
				angle = -Vector2.Angle(forwardVector, -this.transform.right);
				
				angle = 180 + angle + m_IADetectionAngle / 2f;
			}
		}

		for (int i = 0; i <= m_IADetectionRayCount; i++)
		{
			Vector3 rayDirection = GetVectorFromAngle(angle);
			RaycastHit2D[] hits = Physics2D.RaycastAll(this.transform.position, rayDirection, m_IADetectionRange, m_IADetectionLayers);

			for (int y = 0; y< hits.Length; y++)
			{
				if (hits[y].collider.GetComponentInParent<DemonBase>() != null)
				{
					if (hits[y].collider.GetComponentInParent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
					{
						return true;
					}
				}
			}

			if(i == 0)
			{
				Debug.DrawRay(this.transform.position, rayDirection, Color.cyan, 3f);
			}
			else
			{
				Debug.DrawRay(this.transform.position, rayDirection, Color.blue, 3f);
			}

			if (m_IADirectionSpeedModifier > 0)
			{
				angle = angle + angleIncrease;
			}
			else
			{
				angle = angle - angleIncrease;
			}
		}

		return false;
	}

	void IAStartChase()
	{
		m_IACurrentState = EnemyState.Chasing;
		m_IAStopping = false; 
	}

	bool  IACheckStopChaseConditions()
	{
		float distance = Vector3.Distance(this.transform.position, m_IAStartingPos);

		if(distance >= m_maxDistanceChasingFromSpawn)
		{
			return true;
		}

		distance = Mathf.Abs(this.transform.position.x - PosesionManager.Instance.ControlledDemon.transform.position.x);
		if(distance >= m_maxDistanceChasingFromPlayerX)
		{
			return true;
		}

		distance = Mathf.Abs(this.transform.position.y - PosesionManager.Instance.ControlledDemon.transform.position.y);
		if (distance >= m_maxDistanceChasingFromPlayerY)
		{
			return true;
		}
		distance = Vector3.Distance(this.transform.position, PosesionManager.Instance.ControlledDemon.transform.position);
		if (distance >= m_maxDistanceChasingFromPlayer)
		{
			return true;
		}
		
		return false;
	}

	void IAChaseUpdate()
	{
		//Set Char being Chased 

		m_IACharacterBeingChased = PosesionManager.Instance.ControlledDemon.transform;

		//Check Direction Of Movement

		float xDistance = m_IACharacterBeingChased.position.x - this.transform.position.x;

		if (xDistance == 0)
		{
			m_IADirectionSpeedModifier = 0;
		}
		else if (xDistance < 0)
		{
			m_IADirectionSpeedModifier = -1;
		}
		else if (xDistance > 0)
		{
			m_IADirectionSpeedModifier = 1;
		}

		//Move

		m_IAVelocity = m_IACmpRb.velocity;
		m_IAcurrentSpeed = m_IAcurrentSpeed + m_IAcceleration * Time.deltaTime * m_IADirectionSpeedModifier;

		if (m_IAcurrentSpeed * m_IADirectionSpeedModifier > m_IAMaxSpeed)
		{
			m_IAcurrentSpeed = m_IAMaxSpeed * m_IADirectionSpeedModifier;
		}

		m_IACmpRb.velocity = new Vector3(m_IAcurrentSpeed, m_IAVelocity.y, 0);
	}

	void IAGoBackToPatrol()
	{
		m_IACurrentState = EnemyState.Patrol;

		IASetNextPatrolPoint();
	}

	void IAPatrolUpdate()
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

		m_IAVelocity = m_IACmpRb.velocity;

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

		m_IACmpRb.velocity = new Vector3(m_IAcurrentSpeed, m_IAVelocity.y, 0);
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

		float xDistance = m_IAPatrolPoints[m_IACurrentPatrolPoint].position.x - this.transform.position.x;
		
		if (xDistance <= 0)
		{
			m_IADirectionSpeedModifier = -1;
		}
		else if (xDistance > 0)
		{
			m_IADirectionSpeedModifier = 1;
		}
	}

	#endregion IA
}
