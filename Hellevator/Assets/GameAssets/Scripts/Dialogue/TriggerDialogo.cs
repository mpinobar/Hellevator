using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogo : TriggerEvent
{
	[SerializeField] private DialogueStart questDialogue = null;
	
	//[SerializeField] private bool m_conversationAtBeginingOfLevel = false;
	private bool conversationStarted = false;

	protected void Start()
	{
		InputManager.Instance.OnInteract += Event;
		m_startsOnEnter = true;
	}

	protected override void Event()
	{
		if (!conversationStarted && isInTrigger)
		{
			PossessionManager.Instance.ControlledDemon.CanMove = false;
			DialogueManager.Instance.PressXToTalk.SetActive(false);
			questDialogue.StartTalking(this);
			conversationStarted = true;
		}
		else if(conversationStarted && isInTrigger)
		{
			DialogueManager.Instance.NextSentence();
		}
	}

	public override void DestroyTrigger()
	{
		InputManager.Instance.IsInInteactionTrigger = false;
		isInTrigger = false;
		Destroy(this.gameObject);
	}
}
