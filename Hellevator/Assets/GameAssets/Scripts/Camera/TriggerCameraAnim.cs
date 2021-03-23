using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraAnim : MonoBehaviour
{
	[SerializeField] private Animator m_camAnimator = null;
	[SerializeField] private string m_animTriggerName = "";

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.TryGetComponent<DemonBase>(out DemonBase demon))
		{
			m_camAnimator.SetTrigger(m_animTriggerName);
		}
	}
}
