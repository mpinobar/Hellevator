using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Video;

public class CameraManager : TemporalSingleton<CameraManager>
{
	[SerializeField] private CinemachineVirtualCamera m_currentCamera = null;
	private Transform m_currentCameraFocus = null;

	[SerializeField] private FadeManager m_fadeManager;
	    
    private Transform m_currentFocus = null;

	Camera m_mainCamera;
	private float m_startingOrtographicSize = 0f;
	public float StartingOrtographicSize { get => m_startingOrtographicSize; }
    public CinemachineVirtualCamera CurrentCamera { get => m_currentCamera; set => m_currentCamera = value; }
    public Transform ElVacio { get => elVacio; set => elVacio = value; }

	public Camera MainCamera
    {
        get
        {
            if (m_mainCamera == null)
            {
                m_mainCamera = Camera.main;
            }
            return m_mainCamera;
        }
    }

    [SerializeField] private Transform elVacio = null;

    private bool laCamaraMurio = false;

    ParalaxManager m_parallaxManager;

    [SerializeField] GameObject m_UIEffects;

    [Header("Shake")]
    [SerializeField] float m_lightShakeAmplitude = 12f;
    [SerializeField] float m_lightShakeDuration = 0.5f;

    [SerializeField] float m_medShakeAmplitude = 30f;
    [SerializeField] float m_medShakeDuration = 0.5f;

    [SerializeField] float m_heavyShakeAmplitude = 80f;
    [SerializeField] float m_heavyShakeDuration = 1f;

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
        }
		
		m_parallaxManager = GetComponent<ParalaxManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        SetupParallax();
		
		m_startingOrtographicSize = m_currentCamera.m_Lens.OrthographicSize;
	}
    public void SetupParallax()
    {
        m_parallaxManager.SetUpSceneParalax();
    }

	/// <summary>
	/// Changes the focus of the player camera to this object.
	/// </summary>
	/// <param name="newCameraFocus"> Requieres a Tranform of the new object to focus</param>
	public void ChangeFocusOfCurrentActiveCameraTo(Transform newCameraFocus)
    {
        if (newCameraFocus != null)
        {
			m_currentCameraFocus = newCameraFocus;

			m_currentCamera.LookAt = m_currentCameraFocus;
            m_currentCamera.Follow = m_currentCameraFocus;

        }
    }	

	public void SetCurrentLiveCamera(CinemachineVirtualCamera newLiveCamera)
	{
		newLiveCamera.Priority = 1;
		m_currentCamera.Priority = 0;

		m_currentCamera = newLiveCamera;
		ChangeFocusOfCurrentActiveCameraTo(m_currentCameraFocus);

	}

	#region CameraEffects
	/// <summary>
	/// Change the ortographic size of the current camera
	/// </summary>
	/// <param name="newOrtographicSize"></param>
	public void ChangeOrtographicSizOfCurrentCamera(float newOrtographicSize)
    {
        m_currentCamera.m_Lens.OrthographicSize = newOrtographicSize;
    }
	
	public void SetUnlimitedSoftZone(bool isUnlimited)
	{
		m_currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_UnlimitedSoftZone = isUnlimited;
	}
	#endregion

	#region CameraShakes
	public void CameraShakeLight()
	{
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_lightShakeAmplitude;
		StopAllCoroutines();
		StartCoroutine(CameraShake(noise, m_lightShakeDuration));
	}

	public void CameraShakeMedium()
	{
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_medShakeAmplitude;
		StopAllCoroutines();
		StartCoroutine(CameraShake(noise, m_medShakeDuration));
	}
	public void CameraShakeMediumWithDelay(float time)
	{
		StartCoroutine(ShakeMediumAndDelay(time));
	}

	IEnumerator ShakeMediumAndDelay(float time)
	{
		yield return new WaitForSeconds(time);
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_medShakeAmplitude;
		StopAllCoroutines();
		StartCoroutine(CameraShake(noise, m_medShakeDuration));
	}

	public void CameraShakeHeavyWithDelay(float time)
	{
		StartCoroutine(ShakeHeavyAndDelay(time));
	}

	IEnumerator ShakeHeavyAndDelay(float time)
	{
		yield return new WaitForSeconds(time);
		CinemachineBasicMultiChannelPerlin noise = m_currentCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
		noise.m_AmplitudeGain = m_heavyShakeAmplitude;
		StopAllCoroutines();
		StartCoroutine(CameraShake(noise, m_heavyShakeDuration));
	}

	/// <summary>
	/// Hace shake de la camara y para al cabo de un tiempo
	/// </summary>
	/// <param name="noiseCmp">El componente de ruido de la camara</param>
	/// <param name="time">El tiempo que tarda en parar el shake</param>
	/// <returns></returns>
	private IEnumerator CameraShake(CinemachineBasicMultiChannelPerlin noiseCmp, float time)
	{
		float timeRemaining = time;
		//float amp = noiseCmp.m_AmplitudeGain;
		while (timeRemaining > 0)
		{
			timeRemaining -= Time.unscaledDeltaTime;
			noiseCmp.m_AmplitudeGain = Mathf.Lerp(noiseCmp.m_AmplitudeGain, 0, (1 - timeRemaining / time));
			yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
		}
		noiseCmp.m_AmplitudeGain = 0f;
	}
	#endregion

	#region VideoPlayStop
	IEnumerator InAndOut()
	{
		bool goingOut = true;
		float variance = 0f;
		float maxVariance = 0.25f;
		float speed = 0.08f;
		while (true)
		{
			if (goingOut)
			{
				CurrentCamera.m_Lens.OrthographicSize += Time.deltaTime * speed;
				variance += Time.deltaTime * speed;
				if (variance >= maxVariance)
				{
					variance = 0f;
					goingOut = !goingOut;
				}
			}
			else
			{
				CurrentCamera.m_Lens.OrthographicSize -= Time.deltaTime * speed;
				variance += Time.deltaTime * speed;
				if (variance >= maxVariance)
				{
					variance = 0f;
					goingOut = !goingOut;
				}
			}
			yield return null;
		}

	}

	public void PlayVideo(VideoClip video)
	{
		VideoPlayer vPlayer = MainCamera.GetComponent<VideoPlayer>();
		if (!vPlayer)
		{
			Debug.LogError("NO HAY VIDEO PLAYER EN LA CAMARA. HAY QUE AÑADIR UNO");
			return;
		}
		StartCoroutine(ShowVideo(video));
	}

	IEnumerator ShowVideo(VideoClip video)
	{
		VideoPlayer vPlayer = MainCamera.GetComponent<VideoPlayer>();
		FadeManager.IsRestarting = false;

		yield return FadeInAndOut();

		if (vPlayer)
		{
			InputManager.Instance.ReleasePlayerInput();
			vPlayer.renderMode = VideoRenderMode.CameraNearPlane;
			vPlayer.clip = video;
			vPlayer.Play();
			StartCoroutine(StopPlayingVideo((float)video.length));
		}
	}

	private IEnumerator StopPlayingVideo(float time)
	{
		yield return new WaitForSeconds(time);
		FadeIn();
		while (FadeManager.IsInTransition)
		{
			yield return null;
		}
		MainCamera.GetComponent<VideoPlayer>().clip = null;
		FadeOut();
		InputManager.Instance.RegainPlayerInput();
	}
	#endregion

	#region UIEffects
	public void ShowUIEffects()
	{
		m_UIEffects.SetActive(true);
	}

	public void HideUIEffects()
	{
		m_UIEffects.SetActive(false);
	}
	#endregion

	#region CameraFade
	public void FadeIn()
	{
		m_fadeManager.StartFadingIn();
	}
	public void FadeOut()
	{
		m_fadeManager.StartFadingOut();
	}
	IEnumerator FadeInAndOut()
	{
		//Debug.LogError("Fading out and in");
		FadeIn();
		while (FadeManager.IsInTransition)
		{
			yield return null;
		}
		FadeOut();
	}

	#endregion

	#region DEPRECATED
	/// <summary>
	/// 
	/// </summary>
	/// <param name="isChangingToPlayerCam">True if new camera is the player camera, false if new camera is different from player camera</param>
	/// <param name="newCam"> Only necessary is newCam different from player camera</param>
	//public void ChangeCameraPriority(bool isChangingToPlayerCam, CinemachineVirtualCamera newCam)
	//{
	//	if (isChangingToPlayerCam)
	//	{
	//		if (m_currentCamera != m_playerCamera)
	//		{
	//			m_playerCamIsCurrentCam = true;
	//			m_playerCamera.Priority = m_cameraHighPriorityValue;

	//			if (m_cameraWithHigherPriority != null)
	//			{
	//				m_cameraWithHigherPriority.Priority = m_cameraLowPriorityValue;
	//				m_cameraWithHigherPriority = null;
	//			}

	//			m_currentCamera = m_playerCamera;
	//		}
	//	}
	//	else
	//	{
	//		if (newCam != m_currentCamera)
	//		{
	//			m_cameraWithHigherPriority = newCam;
	//			m_currentCamera = newCam;

	//			m_playerCamera.Priority = m_cameraLowPriorityValue;
	//			m_cameraWithHigherPriority.Priority = m_cameraHighPriorityValue;

	//		}
	//	}
	//}
	#endregion
}
