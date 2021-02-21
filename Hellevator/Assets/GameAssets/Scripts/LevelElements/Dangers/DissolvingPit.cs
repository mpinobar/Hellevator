using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingPit : MonoBehaviour
{

    [SerializeField] AudioClip m_acidClip;
    [SerializeField] Animator m_associatedFryingDemon;
    SpriteRenderer m_spriteRenderer;
    [SerializeField] ParticleSystem m_burstParticles;
    [SerializeField] ParticleSystem m_bubblesParticles;
    [SerializeField] float m_immersionSpeed = 0.5f;
    [SerializeField] float m_timeToDestroy = 2.5f;
    bool m_demonEatingAnimation;
    bool m_clipIsPlaying = false;
    float m_currentClipTimer = 0f;
    float m_clipDuration = 0f;
    [SerializeField] bool m_disablesSkills;

    private void Start()
    {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        m_clipDuration = m_acidClip.length;
    }
    private void Update()
    {
        if (m_currentClipTimer > 0)
        {
            m_currentClipTimer -= Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();

        if (cmpDemon != null)
        {
            if (m_disablesSkills)
            {
                GetComponent<SkillDisabler>()?.DisableSkills(collision);
            }
            if (m_spriteRenderer)
            {
                if (m_spriteRenderer.isVisible)
                {
                    if (m_currentClipTimer <= 0)
                    {
                        AudioManager.Instance.PlayAudioSFX(m_acidClip, false, 0.65f);
                        m_currentClipTimer = m_clipDuration;
                    }
                }
            }
            //the torso has entered the pit while being a ragdoll
            if (collision == cmpDemon.RagdollLogicCollider)
            {
                if (m_associatedFryingDemon)
                    StartCoroutine(EatAnimation());
                cmpDemon.Die(true);
                //Destroy(cmpDemon.gameObject);
                StartCoroutine(SlowImmersion(cmpDemon));

                PlayParticles();
            }
            else if (collision == cmpDemon.PlayerCollider)
            {
                if (m_associatedFryingDemon)
                    StartCoroutine(EatAnimation());
                cmpDemon.Die(false);
                StartCoroutine(SlowImmersion(cmpDemon));
                PlayParticles();
            }
        }
        else
        {
            RagdollLogicalCollider ragdollCollider = collision.transform.GetComponent<RagdollLogicalCollider>();
            if (ragdollCollider)
            {
                if (m_associatedFryingDemon)
                    StartCoroutine(EatAnimation());

                StartCoroutine(SlowImmersion(ragdollCollider));

                PlayParticles();
            }
        }
    }

    private void PlayParticles()
    {
        if (m_burstParticles)
            m_burstParticles.Play();
        if (m_bubblesParticles)
            m_bubblesParticles.Play();
    }

    IEnumerator EatAnimation()
    {
        if (!m_demonEatingAnimation)
        {

            //MusicManager.Instance.PlayAudioSFX(m_acidClip, false, 0.65f);
            m_associatedFryingDemon.SetBool("Eat", true);
            m_demonEatingAnimation = true;

            yield return new WaitForSeconds(0.25f);
            m_demonEatingAnimation = false;
            m_associatedFryingDemon.SetBool("Eat", false);
        }
    }

    private IEnumerator SlowImmersion(DemonBase characterToImmerse)
    {
        characterToImmerse.IsInDanger = true;
        characterToImmerse.IsPossessionBlocked = true;
        float timeToDestroy = m_timeToDestroy;
        characterToImmerse.SetRagdollNewGravity(0);
        ((BasicZombie)characterToImmerse).ResetRagdollVelocity();
        ((BasicZombie)characterToImmerse).ResetVelocity();
        Vector3 initialPoint = characterToImmerse.Torso.transform.position;
        Vector3 endPoint = initialPoint - Vector3.up*2f;

        Transform characterTransform = characterToImmerse.Torso.transform;

        SpriteRenderer [] rends  = characterToImmerse.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingLayerName = "Checkpoint";
        }

        while (timeToDestroy > 0 && characterTransform != null)
        {
            characterTransform.position = Vector3.Lerp(characterTransform.position, endPoint, Time.deltaTime * m_immersionSpeed);
            timeToDestroy -= Time.deltaTime;
            yield return null;
        }

        if (characterToImmerse)
            Destroy(characterToImmerse.gameObject);

    }

    private IEnumerator SlowImmersion(RagdollLogicalCollider ragdollCollider)
    {
        DemonBase characterToImmerse = ragdollCollider.ParentDemon;

        characterToImmerse.IsInDanger = true;
        characterToImmerse.IsPossessionBlocked = true;
        float timeToDestroy = m_timeToDestroy;
        characterToImmerse.SetRagdollNewGravity(0);
        ((BasicZombie)characterToImmerse).ResetRagdollVelocity();
        ((BasicZombie)characterToImmerse).ResetVelocity();

        Transform characterTransform = characterToImmerse.Torso.transform;

        Vector3 initialPoint = characterTransform.position;
        Vector3 endPoint = initialPoint - Vector3.up*2f;

        SpriteRenderer [] rends  = characterToImmerse.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].sortingLayerName = "Checkpoint";
        }

        while (timeToDestroy > 0 && characterToImmerse != null)
        {
            characterToImmerse.IsInDanger = true;
            characterToImmerse.IsPossessionBlocked = true;
            characterTransform.position = Vector3.Lerp(characterTransform.position, endPoint, Time.deltaTime * m_immersionSpeed);
            timeToDestroy -= Time.deltaTime;
            yield return null;
        }

        if (characterToImmerse.gameObject)
            Destroy(characterToImmerse.gameObject);
        if (ragdollCollider.gameObject)
            Destroy(ragdollCollider.transform.root.gameObject);

    }

}
