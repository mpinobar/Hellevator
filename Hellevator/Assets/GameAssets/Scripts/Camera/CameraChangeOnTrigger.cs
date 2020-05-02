using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraChangeOnTrigger : MonoBehaviour
{
    [SerializeField] CameraManager cameraManager;
    [SerializeField] Transform pointToLook;

    [SerializeField] float ortographicSize;

    [SerializeField] bool insideOtherTrigger;
    [SerializeField] GameObject triggerToDesactivate;
    [SerializeField] bool hasToFollowEnemy;
    
    Animator cam1, cam2;

    void Awake()
    {
        cam1 = cameraManager.Camera1.GetComponent<Animator>();
        cam2 = cameraManager.Camera2.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PosesionManager.Instance.ControlledDemon != null)
        {
            if (collision.gameObject == PosesionManager.Instance.ControlledDemon.gameObject)
            {
                if (insideOtherTrigger)
                {
                    triggerToDesactivate.SetActive(false);
                    if (hasToFollowEnemy)
                    {
                        pointToLook = PosesionManager.Instance.ControlledDemon.transform;

                        cameraManager.PointToLook = pointToLook;
                        cameraManager.NewOrtographicSize = ortographicSize;

                        cameraManager.IsTriggerFarCamera = true;

                        cameraManager.Camera1.SetActive(false);
                        cameraManager.Camera2.SetActive(false);

                        cameraManager.ChangeCamTarget();
                    }


                    if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
                    {

                    }
                }

                else
                {
                    if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
                    {
                        print(collision.transform.root.GetComponent<DemonBase>());

                        /*if(cameraManager.Camera1.activeSelf == true)
                        {
                            cam1.SetTrigger("zoom");
                        }
                        else if(cameraManager.Camera2.activeSelf == true)
                        {
                            cam2.SetTrigger("zoom");
                        }*/
                        cameraManager.PointToLook = pointToLook;
                        cameraManager.NewOrtographicSize = ortographicSize;

                        cameraManager.IsTriggerFarCamera = true;

                        cameraManager.Camera1.SetActive(false);
                        cameraManager.Camera2.SetActive(false);

                        cameraManager.ChangeCamTarget();
                    }
                }
            }

        }

		
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.root.GetComponent<DemonBase>() == PosesionManager.Instance.ControlledDemon && !PosesionManager.Instance.PLight.gameObject.activeSelf)
        {

            /*if (cameraManager.Camera1.activeSelf == true)
            {
                cam1.SetTrigger("zoom");
            }
            else if (cameraManager.Camera2.activeSelf == true)
            {
                cam2.SetTrigger("zoom");
            }*/

            cameraManager.IsTriggerFarCamera = false;

            cameraManager.Camera3.SetActive(false);
            cameraManager.Camera4.SetActive(false);

           cameraManager.ChangeCamTarget();
        }
    }
}
