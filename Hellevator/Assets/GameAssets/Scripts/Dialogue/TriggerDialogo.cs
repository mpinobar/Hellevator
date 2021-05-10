using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDialogo : MonoBehaviour
{
	[Tooltip("Dejar vacio si la conversacion comienza con el jugador hablando")]
	[SerializeField] private GameObject m_canvasForDialogue = null;
	[SerializeField] private bool m_playerStartsConversation = false;

	[Space]
	[SerializeField] private GameObject m_canStartConversationIndicator = null; //El gameobject que tenga un indicador de que puedes empezar a hablar con alguien
	
	[Space]
	[SerializeField] private bool m_isRepeatable = false;
	private bool m_hasBeingPlayed = false;

	[Space]
	[SerializeField] protected Dialogue dialogue;
	[SerializeField] List<AudioClip> m_dialogueClipsToPlay;
	[SerializeField] float m_pitchModifier = 1;
	protected void Event()
	{
        if (!m_hasBeingPlayed)
        {
			AudioManager.Instance.DialogueSrc.pitch = m_pitchModifier;
			DialogueManager.Instance.DialogueClipsToPlay = m_dialogueClipsToPlay;
			DialogueManager.Instance.StartTalking(dialogue, m_canvasForDialogue);
			InputManager.Instance.ResetPlayerInput();

			if (!m_isRepeatable)
			{
				m_hasBeingPlayed = true;

			}
		}
	}


	protected void OnTriggerEnter2D(Collider2D collision)
	{
        if (!m_hasBeingPlayed)
        {
			if ((PossessionManager.Instance.ControlledDemon != null) && (collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon))
			{
				if (m_playerStartsConversation)
				{
					m_canvasForDialogue = PossessionManager.Instance.ControlledDemon.GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject;
				}

				InputManager.Instance.IsInInteactionTrigger = true;
				Event();
			}
		}		
	}


	public void DestroyTrigger()
	{
		this.gameObject.SetActive(false);
	}

	public void StartDialogue(GameObject canvas)
    {
		m_canvasForDialogue = canvas;

		InputManager.Instance.IsInInteactionTrigger = true;

		Event();
	}
}
