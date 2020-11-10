using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSplatter : MonoBehaviour
{
    [SerializeField] float m_probSpawnSplatter = 0.15f;
    [SerializeField] GameObject [] m_splatPrefabs;
    ParticleSystem m_particleSystem;
    List<ParticleCollisionEvent> m_events;

    // Start is called before the first frame update
    void Start()
    {
        m_particleSystem = GetComponent<ParticleSystem>();
        m_events = new List<ParticleCollisionEvent>();
    }

    private void OnParticleCollision(GameObject other)
    {
        ParticlePhysicsExtensions.GetCollisionEvents(m_particleSystem, other, m_events);

        for (int i = 0; i < m_events.Count; i++)
        {
            if (Random.value <= m_probSpawnSplatter)
            {
                if(other.GetComponent<SpriteMask>() == null)
                {
                    other.AddComponent<SpriteMask>();
                    other.GetComponent<SpriteMask>().sprite = other.GetComponent<SpriteRenderer>().sprite;
                }
                else
                {
                    Transform splat = Instantiate(m_splatPrefabs[Random.Range(0, m_splatPrefabs.Length)], m_events[i].intersection - m_events[i].normal, Quaternion.Euler(0, 0, Random.value * 360), other.transform).transform;
                    //splat.localScale = new Vector3(1/other.transform.localScale.x, 1 / other.transform.localScale.y,1);
                }
            }
        }
    }
}
