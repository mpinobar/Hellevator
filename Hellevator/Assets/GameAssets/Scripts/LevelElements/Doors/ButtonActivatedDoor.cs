using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonActivatedDoor : ActivatedBase
{	
	[SerializeField] private float m_speed;
	[SerializeField] private Transform m_endPosition;
	private bool m_opening = false;

	
    private void Update()
	{
		if (m_opening)
		{
			transform.position = Vector3.MoveTowards(transform.position, m_endPosition.position, m_speed * Time.deltaTime);
			if((Vector3.Distance(transform.position, m_endPosition.position)) == 0)
			{
				enabled = false;
			}
		}
	}

	/// <summary>
	/// The behaviour of a Door that openes when a preassure plate (type Button) is activated
	/// </summary>
	public override void Activate()
	{
		base.Activate();
		m_opening = true;
		
	}

    public override void ActivateImmediately()
    {
		base.ActivateImmediately();
		transform.position = m_endPosition.position;
		enabled = false;
		
	}
	public void ChangeEndPosition(Vector3 newPosition)
    {
		m_endPosition.position = newPosition;
    }
}
