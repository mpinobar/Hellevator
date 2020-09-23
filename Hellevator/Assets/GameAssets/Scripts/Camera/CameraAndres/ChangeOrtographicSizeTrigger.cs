using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeOrtographicSizeTrigger : MonoBehaviour
{
	[SerializeField] private float m_newOrtographicSize = 0f;

	private void OnTriggerEnter2D(Collider2D collision) //The current ortographic size of the camera changes to m_newOrtographicSize
	{
		if (collision.gameObject == PossessionManager.Instance.ControlledDemon.gameObject)
		{
			CameraManager.Instance.ChangeOrtographicSizOfCurrentCamera(m_newOrtographicSize);
		}
	}

	private void OnTriggerExit2D(Collider2D collision) //Camera goes back to starting ortographic size
	{
		if (collision.gameObject == PossessionManager.Instance.ControlledDemon.gameObject)
		{
			CameraManager.Instance.ChangeOrtographicSizOfCurrentCamera(CameraManager.Instance.StartingOrtographicSize);
		}
	}
}
