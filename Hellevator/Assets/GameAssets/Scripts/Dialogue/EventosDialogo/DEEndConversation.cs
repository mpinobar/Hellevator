using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEEndConversation : DialogEvent
{
    public override void ActivateEvent()
    {
        InputManager.Instance.IsInDialogue = false;
        InputManager.Instance.IsInInteactionTrigger = false;
        DialogueManager.Instance.DeactivateTextAndImage();
    }
}
