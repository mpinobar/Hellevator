using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueStart : MonoBehaviour
{

    [SerializeField] private Dialogue dialogue;
	private int currentDialogue;

    public int CurrentDialogue { get => currentDialogue; set => currentDialogue = value; }
    public Dialogue Dialogue { get => dialogue; set => dialogue = value; }

    public void StartTalking(TriggerEvent triggerer)
    {
        DialogueManager.Instance.StartTalking(dialogue);
		dialogue.Triggerer = triggerer;
    }

}
