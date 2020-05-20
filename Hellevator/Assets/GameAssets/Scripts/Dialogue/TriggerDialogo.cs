using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogo : TriggerEvent
{
	[SerializeField] private DialogueStart questDialogue = null;
	//[SerializeField] private bool m_conversationAtBeginingOfLevel = false;
	private bool conversationStarted = false;

	protected override void Event()
	{
		if (!conversationStarted && isInTrigger)
		{
			DialogueManager.Instance.PressXToTalk.SetActive(false);
			InputManager.Instance.CanMove = false;

			questDialogue.StartTalking(this);
			conversationStarted = true;
		}
		else
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
