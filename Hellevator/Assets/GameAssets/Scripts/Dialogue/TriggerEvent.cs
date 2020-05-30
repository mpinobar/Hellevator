using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerEvent : MonoBehaviour
{
	[SerializeField] protected bool m_startsOnEnter = false;
	protected bool isInTrigger = false;

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if((collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon) && (PosesionManager.Instance.ControlledDemon != null))
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
		if ((collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon) && (PosesionManager.Instance.ControlledDemon != null))
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
