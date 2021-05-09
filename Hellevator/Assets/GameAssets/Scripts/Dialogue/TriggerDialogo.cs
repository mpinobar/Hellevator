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
	[SerializeField] private bool m_conversationStartsInstantly = false;
	private bool m_onTigger = false;

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
			if (m_conversationStartsInstantly && m_onTigger)
			{
				print(gameObject.name);
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

				m_onTigger = true;
				InputManager.Instance.IsInInteactionTrigger = true;

				if (m_conversationStartsInstantly)
				{
					Event();
				}
				else
				{
					m_canStartConversationIndicator.SetActive(true);
				}
			}
		}		
	}

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((PossessionManager.Instance.ControlledDemon != null) && (collision.transform.root.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon))
		{
			InputManager.Instance.IsInInteactionTrigger = false;
			m_onTigger = false;

			if (!m_conversationStartsInstantly)
			{
				m_canStartConversationIndicator.SetActive(false);
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

		m_onTigger = true;
		InputManager.Instance.IsInInteactionTrigger = true;

		Event();
	}
}
