using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayIntroText : MonoBehaviour
{
    [SerializeField] private TriggerDialogo m_dialogo = null;
	[SerializeField] private float m_timeBeforeStartDialogue = 0f;
    [SerializeField] private GameObject m_canvas = null;
	private float m_currentTimer = 0f;
	private bool m_counting = false;
    [SerializeField] private bool m_blockInputImmediatly = true;

    private void Update()
    {
        if (m_counting)
        {
			m_currentTimer -= Time.deltaTime;
            if (m_blockInputImmediatly)
            {
                InputManager.Instance.IsInInteactionTrigger = true;
                InputManager.Instance.ResetPlayerHorizontalInput();
                InputManager.Instance.IsInDialogue = true;
            }

            if (m_currentTimer <= 0)
            {
                InputManager.Instance.IsInInteactionTrigger = true;
                InputManager.Instance.ResetPlayerHorizontalInput();
                InputManager.Instance.IsInDialogue = true;

                if (m_canvas != null)
                {
				    m_dialogo.StartDialogue(m_canvas);
                }
                else
                {
				    m_dialogo.StartDialogue(PossessionManager.Instance.ControlledDemon.GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject);
                }
                m_counting = false;
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
        m_currentTimer = m_timeBeforeStartDialogue;
        m_counting = true;

        if (m_blockInputImmediatly)
        {
            InputManager.Instance.IsInInteactionTrigger = true;
            InputManager.Instance.ResetPlayerHorizontalInput();
            InputManager.Instance.IsInDialogue = true;
        }
    }
}
