using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : TemporalSingleton<CameraManager>
{

	[SerializeField] private CinemachineVirtualCamera m_playerCamera = null;
	private CinemachineVirtualCamera m_cameraWithHigherPriority = null;

	[SerializeField] private CinemachineVirtualCamera m_currentCamera = null;
	[SerializeField] private FadeManager m_fadeManager;
	[SerializeField] private int m_cameraHighPriorityValue = 0;
	[SerializeField] private int m_cameraLowPriorityValue = 0;

	private bool m_playerCamIsCurrentCam = true;

	private Transform m_currentFocus = null;

	private float m_startingOrtographicSize = 0f;

	public float StartingOrtographicSize { get => m_startingOrtographicSize; }
	public CinemachineVirtualCamera CurrentCamera { get => m_currentCamera; set => m_currentCamera = value; }
	public Transform ElVacio { get => elVacio; set => elVacio = value; }
	public CinemachineVirtualCamera PlayerCamera { get => m_playerCamera; set => m_playerCamera = value; }

	[SerializeField] private Transform elVacio = null;

	private bool laCamaraMurio = false;

	ParalaxManager m_parallaxManager;


	[Header("Shake")]
	[SerializeField] float m_lightShakeAmplitude = 12f;
	[SerializeField] float m_lightShakeDuration = 0.5f;

	[SerializeField] float m_medShakeAmplitude = 30f;
	[SerializeField] float m_medShakeDuration = 0.5f;

	public override void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
		}
		else
		{
			Destroy(_instance.gameObject);
			_instance = this;
			//ChangeFocusOfMainCameraTo(PossessionManager.Instance.ControlledDemon.transform);
		}
		m_parallaxManager = GetComponent<ParalaxManager>();
		m_currentCamera = m_playerCamera;
		m_startingOrtographicSize = m_currentCamera.m_Lens.OrthographicSize;
		m_currentCamera.Priority = m_cameraHighPriorityValue;
	}


	// Start is called before the first frame update
	void Start()
    {
        SetupParallax();
    }

    public void SetupParallax()
    {
        m_parallaxManager.SetUpSceneParalax();
    }

	public void FadeIn()
    {
		m_fadeManager.StartFadingIn();
    }
	public void FadeOut()
    {
		m_fadeManager.StartFadingOut();
	}

    /// <summary>
    /// Changes the focus of the player camera to this object.
    /// </summary>
    /// <param name="newCameraFocus"> Requieres a Tranform of the new object to focus</param>
    public void ChangeFocusOfMainCameraTo(Transform newCameraFocus)
	{
		if(newCameraFocus != null)
		{
			m_playerCamera.LookAt = newCameraFocus;
			m_playerCamera.Follow = newCameraFocus;
		}
	}

	public void SetCameraFocus(Transform newCameraFocus)
	{
		if(newCameraFocus != null)
		{
			m_playerCamera.LookAt = newCameraFocus;
			m_playerCamera.Follow = newCameraFocus;

			m_playerCamera.enabled = false;
			m_playerCamera.transform.position = new Vector3(newCameraFocus.transform.position.x, newCameraFocus.transform.position.y, this.transform.position.z);
			m_playerCamera.enabled = true;
		}
	}

	
	/// <summary>
	/// 
	/// </summary>
	/// <param name="isChangingToPlayerCam">True if new camera is the player camera, false if new camera is different from player camera</param>
	/// <param name="newCam"> Only necessary is newCam different from player camera</param>
	public void ChangeCameraPriority(bool isChangingToPlayerCam, CinemachineVirtualCamera newCam)
	{
		if (isChangingToPlayerCam)
		{
			if(m_currentCamera != m_playerCamera)
			{
				m_playerCamIsCurrentCam = true;
				m_playerCamera.Priority = m_cameraHighPriorityValue;

				if (m_cameraWithHigherPriority != null)
				{
					m_cameraWithHigherPriority.Priority = m_cameraLowPriorityValue;
					m_cameraWithHigherPriority = null;
				}

				m_currentCamera = m_playerCamera;
			}
		}
		else
		{
			if(newCam != m_currentCamera)
			{
				m_cameraWithHigherPriority = newCam;
				m_currentCamera = newCam;

				m_playerCamera.Priority = m_cameraLowPriorityValue;
				m_cameraWithHigherPriority.Priority = m_cameraHighPriorityValue;

			}
		}
	}

	/// <summary>
	/// Change the ortographic size of the current camera
	/// </summary>
	/// <param name="newOrtographicSize"></param>
	public void ChangeOrtographicSizOfCurrentCamera(float newOrtographicSize)
	{
		m_currentCamera.m_Lens.OrthographicSize = newOrtographicSize;
	}

	public void CameraShakeLight()
    {
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_lightShakeAmplitude;
		StopAllCoroutines();
		StartCoroutine(StopShaking(noise, m_lightShakeDuration));
    }

	public void CameraShakeMedium()
    {
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_medShakeAmplitude;
		StopAllCoroutines();
        StartCoroutine(StopShaking(noise, m_medShakeDuration));
    }


	private IEnumerator StopShaking(CinemachineBasicMultiChannelPerlin noiseCmp, float time)
    {
		float timeRemaining = time;
		//float amp = noiseCmp.m_AmplitudeGain;
		while (timeRemaining > 0)
        {
			timeRemaining -= Time.deltaTime;
			noiseCmp.m_AmplitudeGain = Mathf.Lerp(noiseCmp.m_AmplitudeGain, 0, (1 - timeRemaining / time));
			yield return null;
        }
		noiseCmp.m_AmplitudeGain = 0f;
    }
}
