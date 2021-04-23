using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEEndConversation : DialogEvent
{
    public override void ActivateEvent()
    {
        DialogueManager.Instance.DeactivateTextAndImage();
    }
}
