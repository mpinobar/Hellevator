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

	[SerializeField] private EnemyState m_IACurrentState = EnemyState.None;
	[SerializeField] private float m_IADetectionRange = 0f;
	[SerializeField] private float m_IADetectionAngle = 0f;
	[SerializeField] private float m_IADetectionRayCount = 0f;
	[SerializeField] private LayerMask m_IADetectionLayers;
	[SerializeField] private LayerMask m_IADetectionLayersForForwardVector;

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

		if(m_IACurrentState != EnemyState.Chasing)
		{
			
		}


		if (Input.GetKeyDown(KeyCode.N))
		{
			IASenseForPlayer();
		}

		//switch (m_IACurrentState)
		//{
		//	case EnemyState.Chasing:
		//		break;
		//	case EnemyState.Patrol:
		//		{
		//			IAPatrolUpdate();
		//		}
		//		break;
		//	case EnemyState.GoingBack:
		//		break;
		//	case EnemyState.None:
		//		break;
		//	default:
		//		break;
		//}
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

	void IASenseForPlayer()
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
			RaycastHit2D hits = Physics2D.Raycast(this.transform.position, rayDirection, m_IADetectionRange, m_IADetectionLayers);

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
	}

	void IAChaseUpdate()
	{
		
	}

	void IAGoBackToPatrol()
	{

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
