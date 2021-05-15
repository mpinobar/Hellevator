using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DESpawnAfterConversation : DialogEvent
{
    [SerializeField] private List<GameObject> m_objectToSpawn;
    [SerializeField] private float m_timeBeforeSpawn = 0.15f;
    [SerializeField] private ParticleSystem m_particles;

    public override void ActivateEvent()
    {
        StartCoroutine(SpawnWithDelay());
    }
    IEnumerator SpawnWithDelay()
    {
        m_particles.Play();
        yield return new WaitForSecondsRealtime(m_timeBeforeSpawn);
        for (int i = 0; i < m_objectToSpawn.Count; i++)
        {
            print("");
            m_objectToSpawn[i].SetActive(true);
        }
    }
}
