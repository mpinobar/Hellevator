using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEventoSierra : MonoBehaviour
{
	[SerializeField] private SierraWithRoute m_sierra;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if(collision.TryGetComponent<DemonBase>(out DemonBase demon))
		{
			m_sierra.StartMoving();
			Destroy(this.gameObject);
		}
	}
}
