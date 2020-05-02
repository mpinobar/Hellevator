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

	[SerializeField] private bool m_hasCutscene = false;
	[SerializeField] private Transform m_luz = null;

    Transform pointToLook;
    float newOrtographicSize;

    bool isCam1;
    bool isCam3;

    bool isTriggerFarCamera;

    public bool IsCam1 { get => isCam1; set => isCam1 = value; }
    public bool IsTriggerFarCamera { get => isTriggerFarCamera; set => isTriggerFarCamera = value; }
    public GameObject Camera1 { get => camera1; set => camera1 = value; }
    public GameObject Camera2 { get => camera2; set => camera2 = value; }
    public GameObject Camera3 { get => camera3; set => camera3 = value; }
    public GameObject Camera4 { get => camera4; set => camera4 = value; }
    public Transform PointToLook { get => pointToLook; set => pointToLook = value; }
    public float NewOrtographicSize { get => newOrtographicSize; set => newOrtographicSize = value; }

    private void Start()
    {
		if(PosesionManager.Instance.ControlledDemon != null)
		{
			vcam1.Follow = PosesionManager.Instance.ControlledDemon.transform;
			vcam1.LookAt = PosesionManager.Instance.ControlledDemon.transform;
		}

        Camera1.SetActive(true);
        Camera2.SetActive(false);
        Camera3.SetActive(false);
        Camera4.SetActive(false);

        IsCam1 = true;
        isCam3 = true;

		if (m_hasCutscene)
		{
			//Activar camara de Cutscene
			vcam1.Follow = m_luz;
			vcam1.LookAt = m_luz;

		}
		else
		{
			ChangeCamTarget();
		}
    }

    public void ChangeCamTarget()
    {
        if (isTriggerFarCamera)
        {
            if (isCam3)
            {
                /*vcam4.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam4.LookAt = PosesionManager.Instance.ControlledDemon.transform;*/

                vcam4.Follow = pointToLook;
                vcam4.LookAt = pointToLook;

                vcam4.m_Lens.OrthographicSize = NewOrtographicSize;

                Camera3.SetActive(false);
                Camera4.SetActive(true);

                isCam3 = false;
            }

            else
            {
                /*vcam3.Follow = PosesionManager.Instance.ControlledDemon.transform;
                vcam3.LookAt = PosesionManager.Instance.ControlledDemon.transform;*/

                vcam3.Follow = pointToLook;
                vcam3.LookAt = pointToLook;

                vcam3.m_Lens.OrthographicSize = NewOrtographicSize;

                Camera3.SetActive(true);
                Camera4.SetActive(false);

                isCam3 = true;
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
