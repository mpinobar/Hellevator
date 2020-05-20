using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventoTerminarConversacion : DialogEvent
{
	public override void ActivateEvent()
	{
		DialogueManager.Instance.DeactivateTextAndImage();
		InputManager.Instance.CanMove = true;
	}
}
