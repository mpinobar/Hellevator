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

    bool isCam1;


    private void Start()
    {
        camera1.SetActive(true);
        camera2.SetActive(false);

        isCam1 = true;

        vcam1.Follow = PosesionManager.Instance.ControlledDemon.transform;
        vcam1.LookAt = PosesionManager.Instance.ControlledDemon.transform;
    }

    public void ChangeCamTarget()
    {
        if (isCam1)
        {
            vcam2.Follow = PosesionManager.Instance.ControlledDemon.transform;
            vcam2.LookAt = PosesionManager.Instance.ControlledDemon.transform;

            camera1.SetActive(false);
            camera2.SetActive(true);

            isCam1 = false;
        }

        else
        {
            vcam1.Follow = PosesionManager.Instance.ControlledDemon.transform;
            vcam1.LookAt = PosesionManager.Instance.ControlledDemon.transform;

            camera1.SetActive(true);
            camera2.SetActive(false);

            isCam1 = true;
        }      
    }
}
