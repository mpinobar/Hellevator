using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorWithWaitTimeInfo
{
	[SerializeField] Transform m_door;
	[SerializeField] float m_waitTimeBeforePreviousEvent;

	public Transform Door { get => m_door; set => m_door = value; }
	public float WaitTimeBeforePreviousEvent { get => m_waitTimeBeforePreviousEvent; set => m_waitTimeBeforePreviousEvent = value; }
}
