using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockMultiTrigger : MonoBehaviour
{
	[SerializeField] GameObject m_canvasTutorial = null;
	[SerializeField] GameObject m_collectible = null;
	bool m_multiIsBeingTaught = false;

	private void Start()
	{
		if(PlayerPrefs.GetInt("MultiIsUnlocked") == 1)
		{
			Destroy(this.gameObject);
		}
		InputManager.Instance.OnInteract += EndTutorial;
	}
	
	private void EndTutorial()
	{
		if (m_multiIsBeingTaught)
		{
			m_canvasTutorial.SetActive(false);
			m_multiIsBeingTaught = false;
			PossessionManager.Instance.ControlledDemon.CanMove = true;
			InputManager.Instance.IsInInteactionTrigger = false;
			Destroy(this.gameObject);
		}
	}



	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
		{
			if (!PossessionManager.Instance.MultiplePossessionIsUnlocked)
			{
				InputManager.Instance.IsInInteactionTrigger = true;
				m_canvasTutorial.SetActive(true);
				m_multiIsBeingTaught = true;
				PossessionManager.Instance.MultiplePossessionIsUnlocked = true;
				PossessionManager.Instance.ControlledDemon.CanMove = false;
				Destroy(m_collectible);
				PlayerPrefs.SetInt("MultiIsUnlocked", 1);
			}
		}
	}
}
