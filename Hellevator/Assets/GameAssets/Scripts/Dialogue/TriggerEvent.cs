using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEvent : MonoBehaviour
{
	[SerializeField] protected bool m_startsOnEnter = false;
	protected bool isInTrigger = false;
	[SerializeField] private GameObject m_canvasForDialogue = null;


	[SerializeField] private Dialogue dialogue;

	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if((PossessionManager.Instance.ControlledDemon != null) && (collision.transform.root.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon) )
		{		
			Event();			
		}
	}

	protected virtual void Event()
	{
		DialogueManager.Instance.StartTalking(dialogue, m_canvasForDialogue);
	}

	public virtual void DestroyTrigger()
	{
		Destroy(this.gameObject);
	}
}
