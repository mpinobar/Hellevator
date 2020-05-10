using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChangeTrigger : MonoBehaviour
{
	[SerializeField] private CinemachineVirtualCamera m_newCamera = null;

	private void OnTriggerEnter2D(Collider2D collision) //The current camera changes to this m_newCamera
	{
		if(collision.gameObject == PosesionManager.Instance.ControlledDemon.gameObject)
		{
			CameraManager.Instance.ChangeCameraPriority(false, m_newCamera);
		}
	}

}
