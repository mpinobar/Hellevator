using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAppearOnTrigger : MonoBehaviour
{
	[SerializeField] GameObject m_objectToActivate = null;
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
		{
			m_objectToActivate.SetActive(true);
		}
	}
	private void OnTriggerExit2D(Collider2D collision)
	{
		if (collision.GetComponent<DemonBase>() == PossessionManager.Instance.ControlledDemon)
		{
			m_objectToActivate.SetActive(false);
		}
	}
}
