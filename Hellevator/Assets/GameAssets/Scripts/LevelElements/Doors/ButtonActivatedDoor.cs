using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivatedDoor : ButtonActivatedBase
{

	[SerializeField] private float m_speed;
	[SerializeField] private Transform m_endPosition;
	private bool m_opening = false;

	private void Update()
	{
		if (m_opening)
		{
			this.transform.position = Vector3.MoveTowards(this.transform.position, m_endPosition.position, m_speed * Time.deltaTime);
			if((Vector3.Distance(this.transform.position, m_endPosition.position)) == 0)
			{
				this.enabled = false;
			}
		}
	}

	/// <summary>
	/// The behaviour of a Door that openes when a preassure plate (type Button) is activated
	/// </summary>
	public override void Activate()
	{
		m_opening = true;
	}
}
