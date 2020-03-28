using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChangeOnTrigger : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;

    private void OnTriggerStay2D(Collider2D collision)
    {
        cameraManager.IsOnTriggerPz = true;

        cameraManager.Camera1.SetActive(false);
        cameraManager.Camera2.SetActive(false);

        cameraManager.ChangeCamTarget();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        cameraManager.IsOnTriggerPz = false;

        cameraManager.Camera3.SetActive(false);
        cameraManager.Camera4.SetActive(false);

        cameraManager.ChangeCamTarget();
    }
}
