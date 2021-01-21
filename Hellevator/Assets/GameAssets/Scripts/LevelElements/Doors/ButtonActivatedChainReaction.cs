using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonActivatedChainReaction : ActivatedBase
{
	private bool m_chainReactionStarted = false;
	[SerializeField] List<ActivatedBase> m_objectsInChain;
	
	[SerializeField] float m_timeToWaitBetween = 0f;
	float m_currentWaitTime = 0f;

	private void Update()
	{
		if (m_chainReactionStarted)
		{
			m_currentWaitTime -= Time.deltaTime;
			if(m_currentWaitTime <= 0f)
			{
				m_objectsInChain[0].Activate();
				m_objectsInChain.Remove(m_objectsInChain[0]);
				m_currentWaitTime = m_timeToWaitBetween;
			}
		}
	}

	/// <summary>
	/// The behaviour of a Door that openes when a preassure plate (type Button) is activated
	/// </summary>
	public override void Activate()
	{
		base.Activate();		
		m_chainReactionStarted = true;
	}

	public override void ActivateImmediately()
	{
		base.ActivateImmediately();
	}
}
