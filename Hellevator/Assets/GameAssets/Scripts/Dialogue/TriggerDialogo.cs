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

	[Space]
	[SerializeField] protected Dialogue dialogue;

	protected void Event()
	{
		if (m_conversationStartsInstantly && m_onTigger)
		{
			DialogueManager.Instance.StartTalking(dialogue, m_canvasForDialogue);
			InputManager.Instance.ResetPlayerInput();

            if (!m_isRepeatable)
            {
				DestroyTrigger();
            }
		}
	}


	protected void OnTriggerEnter2D(Collider2D collision)
	{
		if ((PossessionManager.Instance.ControlledDemon != null) && (collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon))
		{
			if (m_playerStartsConversation)
			{
				m_canvasForDialogue = PossessionManager.Instance.ControlledDemon.GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject;
			}

			m_onTigger = true;
			print(InputManager.Instance.IsInInteactionTrigger);
			InputManager.Instance.IsInInteactionTrigger = true;
			print(InputManager.Instance.IsInInteactionTrigger);

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

	private void OnTriggerExit2D(Collider2D collision)
	{
		if ((PossessionManager.Instance.ControlledDemon != null) && (collision.transform.root.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon))
		{
			print("I exit");
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
