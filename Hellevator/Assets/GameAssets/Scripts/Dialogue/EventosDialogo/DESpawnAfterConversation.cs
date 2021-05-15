using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DESpawnAfterConversation : DialogEvent
{
    [SerializeField] private List<GameObject> m_objectToSpawn;
    [SerializeField] private float m_timeBeforeSpawn = 0.15f;
    [SerializeField] private ParticleSystem m_particles;
    [SerializeField] private AudioClip m_audio;

    public override void ActivateEvent()
    {
        StartCoroutine(SpawnWithDelay());
    }
    IEnumerator SpawnWithDelay()
    {
        if(m_particles != null)
        {
            m_particles.Play();
        }

        if (m_audio != null)
        {
            AudioManager.Instance.PlayAudioSFX(m_audio, false, 2f);
        }
        yield return new WaitForSecondsRealtime(m_timeBeforeSpawn);
        for (int i = 0; i < m_objectToSpawn.Count; i++)
        {
            m_objectToSpawn[i].SetActive(true);
        }
    }
}
