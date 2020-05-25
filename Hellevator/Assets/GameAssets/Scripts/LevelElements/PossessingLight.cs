using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessingLight : MonoBehaviour
{
    [SerializeField] AudioClip m_lightTravelClip;

    bool m_travelling;
    DemonBase m_target;
    DemonBase m_originDemon;
    private float m_initialDistance;
    private float m_distancePercentage;
    AudioSource m_lightSound;
    float m_lastDemonPossessionRange;
    [SerializeField] float m_speed = 3.5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_travelling)
        {
            if(m_target == null)
            {
                m_target = PosesionManager.Instance.LookForNearestDemon(m_lastDemonPossessionRange, transform, m_originDemon);
                if(m_target == null)
                {
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

    public void Begin(DemonBase d, float lastDemonPossessionRange, DemonBase originDemon)
    {
        m_target = d;
        m_originDemon = originDemon;
        m_lastDemonPossessionRange = lastDemonPossessionRange;
        m_travelling = true;
        m_lightSound = MusicManager.Instance.PlayAudioSFX(m_lightTravelClip, false);
        m_initialDistance = Vector2.Distance(transform.position, m_target.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<DemonBase>() == m_target)
        {
            PosesionManager.Instance.PossessNewDemon(m_target);
            m_lightSound.Stop();
        }
    }

}
