using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChangeOnTrigger : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;
    Animator cam1, cam2;

    void Awake()
    {
        cam1 = cameraManager.Camera1.GetComponent<Animator>();
        cam2 = cameraManager.Camera2.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("dentroTrigger");
        if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
        {
            print(collision.transform.root.GetComponent<DemonBase>());

            if(cameraManager.Camera1.activeSelf == true)
            {
                cam1.SetTrigger("zoom");
            }
            else if(cameraManager.Camera2.activeSelf == true)
            {
                cam2.SetTrigger("zoom");
            }

            //cameraManager.IsTriggerFarCamera = true;

            //cameraManager.Camera1.SetActive(false);
            //cameraManager.Camera2.SetActive(false);

            //cameraManager.ChangeCamTarget();
        }            
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
        {
            print("FueraTrigger");

            if (cameraManager.Camera1.activeSelf == true)
            {
                cam1.SetTrigger("zoom");
            }
            else if (cameraManager.Camera2.activeSelf == true)
            {
                cam2.SetTrigger("zoom");
            }

            //cameraManager.IsTriggerFarCamera = false;

            //cameraManager.Camera3.SetActive(false);
            //cameraManager.Camera4.SetActive(false);

            //cameraManager.ChangeCamTarget();
        }
    }
}
