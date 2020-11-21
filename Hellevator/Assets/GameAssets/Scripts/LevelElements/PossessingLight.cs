using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessingLight : MonoBehaviour
{
    [SerializeField] AudioClip m_lightTravelClip;
    [SerializeField] ParticlesPossessionTargetReached m_prefabShockwaveStart;
    [SerializeField] ParticlesPossessionTargetReached m_prefabShockwaveEnd;
    [SerializeField] GameObject m_prefabSmallBurstStart;
    [SerializeField] GameObject m_prefabSmallBurstEnd;

    bool m_travelling;
    DemonBase m_target;
    DemonBase m_originDemon;
    private float m_initialDistance;
    private float m_distancePercentage;
    AudioSource m_lightSound;
    float m_lastDemonPossessionRange;
    [SerializeField] float m_speed = 3.5f;

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            if(m_target == null || m_target.IsPossessionBlocked || m_target.IsInDanger || !m_target.enabled)
            {
                m_target = PossessionManager.Instance.LookForNearestDemon(m_lastDemonPossessionRange, transform, m_originDemon);
                if(m_target == null)
                {
                    Debug.LogError(m_originDemon.name);
                    LevelManager.Instance.StartRestartingLevel();
                }
                else
                {
                    m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
                }
            }
            else
            {
                m_distancePercentage = Vector2.Distance(transform.position, m_target.transform.position) / m_initialDistance;
                m_lightSound.volume = m_distancePercentage * MusicManager.SfxVolume;
                transform.position = Vector3.MoveTowards(transform.position, m_target.Torso.position, m_speed * Time.deltaTime);
            }
        }
        
    }

    public void Begin(DemonBase destinationDemon, float lastDemonPossessionRange, DemonBase originDemon)
    {
        m_target = destinationDemon;
        m_originDemon = originDemon;
        m_lastDemonPossessionRange = lastDemonPossessionRange;
        m_travelling = true;
        m_lightSound = MusicManager.Instance.PlayAudioSFX(m_lightTravelClip, false);
        m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
        Instantiate(m_prefabShockwaveStart, transform.position, Quaternion.identity);
        Instantiate(m_prefabSmallBurstStart, transform.position, Quaternion.identity);
        //Debug.LogError("Possessing light towards " + destinationDemon.name + " from " + originDemon);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<DemonBase>() == m_target)
        {
            if(m_target.transform.parent != null)
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
    }

}
