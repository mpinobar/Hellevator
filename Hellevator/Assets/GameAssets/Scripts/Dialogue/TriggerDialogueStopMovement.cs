using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogueStopMovement : TriggerEvent
{
	[SerializeField] private DialogueStart questDialogue = null;
	
	private bool conversationStarted = false;

	protected void Start()
	{
		InputManager.Instance.OnInteract += Event;
	}

	protected override void Event()
	{
		if (!conversationStarted && isInTrigger)
		{
			PosesionManager.Instance.ControlledDemon.StopMovement();
			DialogueManager.Instance.PressXToTalk.SetActive(false);
			InputManager.Instance.CanMove = false;

			questDialogue.StartTalking(this);
			conversationStarted = true;
		}
		else if (conversationStarted && isInTrigger)
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
