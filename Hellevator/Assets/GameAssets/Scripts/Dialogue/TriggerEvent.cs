using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerEvent : MonoBehaviour
{
	[SerializeField] protected bool m_startsOnEnter = false;
	protected bool isInTrigger = false;

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if((PossessionManager.Instance.ControlledDemon != null) && (collision.transform.root.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon) )
		{			
			isInTrigger = true;
			InputManager.Instance.IsInInteactionTrigger = true;
			if (m_startsOnEnter)
			{
				Event();
			}
			else
			{
				DialogueManager.Instance.PressXToTalk.SetActive(true);
			}
		}
	}
	protected void OnTriggerExit2D(Collider2D collision)
	{
		if ((PossessionManager.Instance.ControlledDemon != null) &&(collision.transform.root.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon))
		{
			InputManager.Instance.IsInInteactionTrigger = false;
			DialogueManager.Instance.PressXToTalk.SetActive(false);
			isInTrigger = false;
		}
	}

	protected abstract void Event();
	
	public virtual void DestroyTrigger()
	{
		
	}
}
