using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Experimental.Rendering.Universal;

public class PossessingLight : MonoBehaviour
{
    [SerializeField] AudioClip m_lightTravelClip;
    [SerializeField] ParticlesPossessionTargetReached m_prefabShockwaveStart;
    [SerializeField] ParticlesPossessionTargetReached m_prefabShockwaveEnd;
    [SerializeField] GameObject m_prefabSmallBurstStart;
    [SerializeField] GameObject m_prefabSmallBurstEnd;
    [SerializeField] AnimationCurve m_sidewaysMovement;
    [SerializeField] AnimationCurve m_forwardMovement;
    float m_forwardMovementTimer;
    [SerializeField] float m_zigZagMovementAmplitude;
    [SerializeField] LayerMask m_lightTravelPoints = 1<<19;
    [SerializeField] bool m_pausesTime;
    bool m_travelling;
    DemonBase m_target;
    DemonBase m_originDemon;
    private float m_initialDistance;
    private float m_distancePercentage;
    AudioSource m_lightSound;
    float m_lastDemonPossessionRange;
    [SerializeField] float m_speed = 3.5f;
    //[SerializeField] int m_framesPerAnimation = 4;

    Vector3 m_desiredPosition;
    Vector3 m_sidewaysVector;
    Vector3 m_lastPosition;
    Vector3 m_initialPosition;
    Transform m_lightTarget;
    private bool m_travellingToLightPoint;

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            if ((m_target == null || m_target.IsPossessionBlocked || m_target.IsInDanger || !m_target.enabled) && !m_travellingToLightPoint)
            {
                m_target = PossessionManager.Instance.LookForNearestDemon(m_lastDemonPossessionRange, transform, m_originDemon);
                if (m_target == null)
                {
                    if(m_lightSound)
                    m_lightSound.Stop();
                    LevelManager.Instance.StartRestartingLevelWithDelay();
                }
                else
                {
                    m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
                }
            }
            else
            {
                //m_desiredPosition = Vector3.Lerp(m_desiredPosition, m_target.Torso.position, m_speed * Time.deltaTime * Mathf.Max(0.5f, m_distancePercentage));
                if (m_travellingToLightPoint && m_lightTarget)
                {
                    //m_forwardMovementTimer += Time.deltaTime * m_speed;

                    //m_desiredPosition = m_initialPosition + (m_lightTarget.position - m_initialPosition) * m_forwardMovement.Evaluate(m_forwardMovementTimer);

                    //m_distancePercentage = Vector2.Distance(m_desiredPosition, m_lightTarget.position) / m_initialDistance;
                    //m_lightSound.volume = m_distancePercentage * AudioManager.SfxVolume;
                    ////transform.up = -(m_target.Torso.position - transform.position);

                    //transform.position = m_desiredPosition + m_sidewaysVector * m_sidewaysMovement.Evaluate(/*m_distancePercentage*/m_forwardMovementTimer) * m_zigZagMovementAmplitude /** m_distancePercentage*/;
                    ////transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);

                    //transform.up = m_lastPosition - transform.position;
                    //m_lastPosition = transform.position;

                    if (Vector3.Distance(m_desiredPosition, m_lightTarget.position) < 1.5f)
                    {
                        //m_travellingToLightPoint = false;
                        //transform.up = (m_target.Torso.position - transform.position);
                        //m_sidewaysVector = -transform.right;
                        //m_initialPosition = transform.position;
                        //m_forwardMovementTimer = 0f;
                        //m_desiredPosition = transform.position;
                        //m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
                        KillLightAndRestart();
                    }
                }
                else if (m_target)
                {
                    m_forwardMovementTimer += Time.deltaTime * m_speed;

                    m_desiredPosition = m_initialPosition + (m_target.Torso.transform.position - m_initialPosition) * m_forwardMovement.Evaluate(m_forwardMovementTimer);

                    m_distancePercentage = Vector2.Distance(m_desiredPosition, m_target.Torso.transform.position) / m_initialDistance;
                    m_lightSound.volume = m_distancePercentage * AudioManager.SfxVolume;
                    //transform.up = -(m_target.Torso.position - transform.position);

                    transform.position = m_desiredPosition + m_sidewaysVector * m_sidewaysMovement.Evaluate(/*m_distancePercentage*/m_forwardMovementTimer) * m_zigZagMovementAmplitude /** m_distancePercentage*/;
                    //transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);

                    transform.up = m_lastPosition - transform.position;
                    m_lastPosition = transform.position;

                    if (Vector3.Distance(m_desiredPosition, m_target.Torso.position) < 1.5f)
                    {
                        EndLightWithCharacterPossession();
                    }
                }

                
            }
        }

    }

    public void KillLightAndRestart()
    {        
        enabled = false;
        m_travellingToLightPoint = false;
        m_originDemon.Torso.parent = null;
        m_originDemon.transform.position = transform.position;
        m_originDemon.PlayTrueDeathEffects();
        LevelManager.Instance.StartRestartingLevelWithDelay();
        m_lightSound.Stop();
        AudioManager.Instance.StopSFX(m_lightTravelClip);
    }
    private void OnDisable()
    {
        m_lightSound.Stop();
        AudioManager.Instance.StopSFX(m_lightTravelClip);
    }

    public void Begin(DemonBase destinationDemon, float lastDemonPossessionRange, DemonBase originDemon)
    {
        m_forwardMovementTimer = 0f;
        m_target = destinationDemon;
        m_originDemon = originDemon;
        m_lastDemonPossessionRange = lastDemonPossessionRange;
        m_travelling = true;
        m_desiredPosition = transform.position;
        m_lightSound = AudioManager.Instance.PlayAudioSFX(m_lightTravelClip, false);
        m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);

        //Collider2D col = Physics2D.OverlapCircle(transform.position,m_initialDistance,m_lightTravelPoints);
        bool col = false;
        if (col)
        {
            //m_lightTarget = col.transform;
            //m_travellingToLightPoint = true;
            //transform.up = (m_lightTarget.position - transform.position);
            //m_sidewaysVector = -transform.right;
            //m_initialPosition = transform.position;
        }
        else
        {
            transform.up = (m_target.Torso.position - transform.position);
            m_sidewaysVector = -transform.right;
            m_initialPosition = transform.position;
        }
        m_lastPosition = transform.position;


        Instantiate(m_prefabShockwaveStart, transform.position, Quaternion.identity);
        Instantiate(m_prefabSmallBurstStart, transform.position, Quaternion.identity);

        if (m_pausesTime)
        {
            Time.timeScale = 0f;
            StartCoroutine(PauseTimeLookForAvailableCharacter());
        }
        //Debug.LogError("Possessing light towards " + destinationDemon.name + " from " + originDemon);

        //StartCoroutine(AnimationLight());

    }

    //private IEnumerator AnimationLight()
    //{
    //    int frames = 0;
    //    int currentIndex = 0;

    //    while (currentIndex < (transform.childCount - 1))
    //    {
    //        frames++;

    //        if (frames % m_framesPerAnimation == 0)
    //        {
    //            transform.GetChild(currentIndex).gameObject.SetActive(false);
    //            currentIndex++;
    //            transform.GetChild(currentIndex).gameObject.SetActive(true);
    //        }
    //        yield return new WaitForSeconds(Time.deltaTime);
    //    }
    //    m_travelling = true;
    //    m_lightSound = MusicManager.Instance.PlayAudioSFX(m_lightTravelClip, false);
    //}

    private IEnumerator PauseTimeLookForAvailableCharacter()
    {
        //ParticleSystem[] particles = GetComponentsInChildren<ParticleSystem>();
        //for (int i = 0; i < particles.Length; i++)
        //{
        //    ParticleSystem.MainModule ps = particles[i].main;
        //    ps.useUnscaledTime = true;
        //}
        while (m_travelling)
        {
            if ((m_target == null || m_target.IsPossessionBlocked || m_target.IsInDanger || !m_target.enabled) && !m_travellingToLightPoint)
            {
                m_target = PossessionManager.Instance.LookForNearestDemon(m_lastDemonPossessionRange, transform, m_originDemon);
                if (m_target == null)
                {
                    m_lightSound.Stop();
                    LevelManager.Instance.StartRestartingLevelWithDelay();
                }
                else
                {
                    m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
                }
            }
            else
            {
                //m_desiredPosition = Vector3.Lerp(m_desiredPosition, m_target.Torso.position, m_speed * Time.deltaTime * Mathf.Max(0.5f, m_distancePercentage));

                if (m_travellingToLightPoint)
                {
                    m_forwardMovementTimer += Time.deltaTime * m_speed;

                    m_desiredPosition = m_initialPosition + (m_lightTarget.position - m_initialPosition) * m_forwardMovement.Evaluate(m_forwardMovementTimer);

                    m_distancePercentage = Vector2.Distance(m_desiredPosition, m_lightTarget.position) / m_initialDistance;
                    m_lightSound.volume = m_distancePercentage * AudioManager.SfxVolume;
                    //transform.up = -(m_target.Torso.position - transform.position);

                    transform.position = m_desiredPosition + m_sidewaysVector * m_sidewaysMovement.Evaluate(/*m_distancePercentage*/m_forwardMovementTimer) * m_zigZagMovementAmplitude /** m_distancePercentage*/;
                    //transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);

                    transform.up = m_lastPosition - transform.position;
                    m_lastPosition = transform.position;

                    if (Vector3.Distance(m_desiredPosition, m_lightTarget.position) < 1.5f)
                    {
                        m_travellingToLightPoint = false;
                        transform.up = (m_target.Torso.position - transform.position);
                        m_sidewaysVector = -transform.right;
                        m_initialPosition = transform.position;
                        m_forwardMovementTimer = 0f;
                        m_desiredPosition = transform.position;
                        m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
                    }
                }
                else
                {
                    m_forwardMovementTimer += Time.deltaTime * m_speed;

                    m_desiredPosition = m_initialPosition + (m_target.Torso.transform.position - m_initialPosition) * m_forwardMovement.Evaluate(m_forwardMovementTimer);

                    m_distancePercentage = Vector2.Distance(m_desiredPosition, m_target.Torso.transform.position) / m_initialDistance;
                    m_lightSound.volume = m_distancePercentage * AudioManager.SfxVolume;
                    //transform.up = -(m_target.Torso.position - transform.position);

                    transform.position = m_desiredPosition + m_sidewaysVector * m_sidewaysMovement.Evaluate(/*m_distancePercentage*/m_forwardMovementTimer) * m_zigZagMovementAmplitude /** m_distancePercentage*/;
                    //transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);

                    transform.up = m_lastPosition - transform.position;
                    m_lastPosition = transform.position;

                    if (Vector3.Distance(m_desiredPosition, m_target.Torso.position) < 1.5f)
                    {
                        EndLightWithCharacterPossession();
                        Time.timeScale = 1f;
                    }
                    
                }
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
       

    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //if (collision.GetComponentInParent<DemonBase>() == m_target)
        //{
        //    EndLightWithCharacterPossession();
        //}
    }

    private void EndLightWithCharacterPossession()
    {
        GetComponent<Collider2D>().enabled = true;
        if (m_target.transform.parent != null)
        {
            Transform parent = m_target.transform.parent;
            m_target.transform.parent = null;
            PossessionManager.Instance.PossessNewDemon(m_target);
            m_target.transform.parent = parent;
            Instantiate(m_prefabShockwaveEnd, transform.position, Quaternion.identity).SetTarget(m_target.Torso);
            Instantiate(m_prefabSmallBurstEnd, transform.position, Quaternion.identity);
            CameraManager.Instance.CameraShakeLight();
        }
        else
        {
            PossessionManager.Instance.PossessNewDemon(m_target);
            Instantiate(m_prefabShockwaveEnd, transform.position, Quaternion.identity).SetTarget(m_target.Torso);
            Instantiate(m_prefabSmallBurstEnd, transform.position, Quaternion.identity);
            CameraManager.Instance.CameraShakeLight();
        }
        m_lightSound.Stop();
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        SoulSyphon syphon = collision.GetComponent<SoulSyphon>();
        if (syphon != null)
        {
            syphon.Spiral(transform);
            //Debug.LogError("asd");
            m_travellingToLightPoint = true;
            m_lightTarget = syphon.transform;
            m_target = null;
        }
        SoulTeleporter teleporter = collision.GetComponent<SoulTeleporter>();
        if (teleporter)
        {
            teleporter.Spiral(transform);
            m_travellingToLightPoint = true;
            m_target = null;
        }
    }

    public void SetFreeFromAttraction()
    {
        m_initialPosition = transform.position;
        m_forwardMovementTimer = 0;
        m_travellingToLightPoint = false;
    }
}
