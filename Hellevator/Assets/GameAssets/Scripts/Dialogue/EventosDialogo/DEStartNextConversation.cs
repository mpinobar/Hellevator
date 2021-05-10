using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEStartNextConversation : DialogEvent
{
    [SerializeField] private TriggerDialogo m_nextDialogue = null;
    [SerializeField] private bool m_nextToTalkIsPlayer = false;
    [Tooltip("Solo hay que poner la referencia si el siguiente en hablar NO es el jugador")]
    [SerializeField] private GameObject m_otherCanvas = null;
    public override void ActivateEvent()
    {
        DialogueManager.Instance.DeactivateTextAndImage();
        InputManager.Instance.IsInInteactionTrigger = true;
        InputManager.Instance.ResetPlayerHorizontalInput();
        InputManager.Instance.IsInDialogue = true;

        if (m_nextToTalkIsPlayer)
        {
            m_nextDialogue.StartDialogue(PossessionManager.Instance.ControlledDemon.GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject);
        }
        else
        {
            m_nextDialogue.StartDialogue(m_otherCanvas);
        }
    }
}
