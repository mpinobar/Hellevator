using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayIntroText : MonoBehaviour
{
    [SerializeField] private TriggerDialogo m_dialogo = null;
	[SerializeField] private float m_timeBeforeStartDialogue = 0f;
	private float m_currentTimer = 0f;
	private bool m_counting = false;

    private void Update()
    {
        if (m_counting)
        {
			m_currentTimer -= Time.deltaTime;
			if(m_currentTimer <= 0)
            {
				m_dialogo.StartDialogue(PossessionManager.Instance.ControlledDemon.GetComponentInChildren<Canvas>().transform.GetChild(0).gameObject);
                m_counting = false;
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
	{
        m_currentTimer = m_timeBeforeStartDialogue;
        m_counting = true;

        InputManager.Instance.IsInInteactionTrigger = true;
        InputManager.Instance.ResetPlayerHorizontalInput();
        InputManager.Instance.IsInDialogue = true;
	}
}
