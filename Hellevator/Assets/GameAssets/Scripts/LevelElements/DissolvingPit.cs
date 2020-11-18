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
    bool m_demonEatingAnimation;
    private void Start()
    {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        if (cmpDemon != null)
        {
            if (m_spriteRenderer)
            {
                if (m_spriteRenderer.isVisible)
                {
                    MusicManager.Instance.PlayAudioSFX(m_acidClip, false, 0.65f);
                }
            }
            //the torso has entered the pit while being a ragdoll
            if (collision == cmpDemon.RagdollLogicCollider)
            {
                if (m_associatedFryingDemon)
                    StartCoroutine(EatAnimation());
                Destroy(cmpDemon.gameObject);
                PlayParticles();
            }
            else if (collision == cmpDemon.PlayerCollider)
            {
                if (m_associatedFryingDemon)
                    StartCoroutine(EatAnimation());
                cmpDemon.Die(false);
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
            MusicManager.Instance.PlayAudioSFX(m_acidClip, false, 0.65f);
            m_associatedFryingDemon.SetBool("Eat", true);
            m_demonEatingAnimation = true;

            yield return new WaitForSeconds(0.25f);
            m_demonEatingAnimation = false;
            m_associatedFryingDemon.SetBool("Eat", false);
        }
    }
}
