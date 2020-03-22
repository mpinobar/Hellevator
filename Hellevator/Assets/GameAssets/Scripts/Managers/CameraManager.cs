using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : TemporalSingleton<CameraManager>
{
    [SerializeField] CinemachineVirtualCamera vcam;

    public void ChangeCamTarget()
    {
        vcam.Follow = PosesionManager.Instance.ControlledDemon.transform;
        vcam.LookAt = PosesionManager.Instance.ControlledDemon.transform;
    }
}
