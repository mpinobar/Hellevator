using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Video;

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
    Camera m_mainCamera;
    public float StartingOrtographicSize { get => m_startingOrtographicSize; }
    public CinemachineVirtualCamera CurrentCamera { get => m_currentCamera; set => m_currentCamera = value; }
    public Transform ElVacio { get => elVacio; set => elVacio = value; }
    public CinemachineVirtualCamera PlayerCamera { get => m_playerCamera; set => m_playerCamera = value; }
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
        //StartCoroutine(InAndOut());
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
        if (newCameraFocus != null)
        {
            m_playerCamera.LookAt = newCameraFocus;
            m_playerCamera.Follow = newCameraFocus;
        }
    }

    public void SetUnlimitedSoftZone(bool isUnlimited)
    {
        m_currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>().m_UnlimitedSoftZone = isUnlimited;
    }

    public void SetCameraFocus(Transform newCameraFocus)
    {
        if (newCameraFocus != null)
        {
            m_playerCamera.LookAt = newCameraFocus;
            m_playerCamera.Follow = newCameraFocus;

            m_playerCamera.enabled = false;
            m_playerCamera.transform.position = new Vector3(newCameraFocus.transform.position.x, newCameraFocus.transform.position.y, this.transform.position.z);
            m_playerCamera.enabled = true;
        }
    }

    public void ShowUIEffects()
    {
        m_UIEffects.SetActive(true);
    }

    public void HideUIEffects()
    {
        m_UIEffects.SetActive(false);
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
            if (m_currentCamera != m_playerCamera)
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
            if (newCam != m_currentCamera)
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
            timeRemaining -= Time.deltaTime;
            noiseCmp.m_AmplitudeGain = Mathf.Lerp(noiseCmp.m_AmplitudeGain, 0, (1 - timeRemaining / time));
            yield return null;
        }
        noiseCmp.m_AmplitudeGain = 0f;
    }
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
                PlayerCamera.m_Lens.OrthographicSize += Time.deltaTime * speed;
                variance += Time.deltaTime * speed;
                if (variance >= maxVariance)
                {
                    variance = 0f;
                    goingOut = !goingOut;
                }
            }
            else
            {
                PlayerCamera.m_Lens.OrthographicSize -= Time.deltaTime * speed;
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
}
