using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : TemporalSingleton<CameraManager>
{
    [SerializeField] CinemachineVirtualCamera vcam1;
    [SerializeField] CinemachineVirtualCamera vcam2;
    [SerializeField] GameObject camera1;
    [SerializeField] GameObject camera2;

    [SerializeField] CinemachineVirtualCamera vcam3;
    [SerializeField] CinemachineVirtualCamera vcam4;
    [SerializeField] GameObject camera3;
    [SerializeField] GameObject camera4;

    bool isCam1;
    bool isCam3;

    bool isTriggerFarCamera;

    public bool IsCam1 { get => isCam1; set => isCam1 = value; }
    public bool IsTriggerFarCamera { get => isTriggerFarCamera; set => isTriggerFarCamera = value; }
    public GameObject Camera1 { get => camera1; set => camera1 = value; }
    public GameObject Camera2 { get => camera2; set => camera2 = value; }
    public GameObject Camera3 { get => camera3; set => camera3 = value; }
    public GameObject Camera4 { get => camera4; set => camera4 = value; }

    private void Start()
    {
        vcam1.Follow = PosesionManager.Instance.ControlledDemon.transform;
        vcam1.LookAt = PosesionManager.Instance.ControlledDemon.transform;

        Camera1.SetActive(true);
        Camera2.SetActive(false);
        Camera3.SetActive(false);
        Camera4.SetActive(false);

        IsCam1 = true;
        isCam3 = true;

    }

    public void ChangeCamTarget()
    {
        if (isTriggerFarCamera)
        {
            if (isCam3)
            {
                vcam4.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam4.LookAt = PosesionManager.Instance.ControlledDemon.transform;

                Camera3.SetActive(false);
                Camera4.SetActive(true);

                isCam3 = false;
            }

            else
            {
                vcam3.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam3.LookAt = PosesionManager.Instance.ControlledDemon.transform;

                Camera3.SetActive(true);
                Camera4.SetActive(false);

                IsCam1 = true;
            }
        }

        else
        {
            if (IsCam1)
            {
                vcam2.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam2.LookAt = PosesionManager.Instance.ControlledDemon.transform;

                Camera1.SetActive(false);
                Camera2.SetActive(true);

                IsCam1 = false;
            }

            else
            {
                vcam1.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam1.LookAt = PosesionManager.Instance.ControlledDemon.transform;

                Camera1.SetActive(true);
                Camera2.SetActive(false);

                IsCam1 = true;
            }
        }        
    }
}
