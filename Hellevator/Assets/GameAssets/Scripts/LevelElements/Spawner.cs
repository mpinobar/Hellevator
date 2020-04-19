using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] DemonBase m_demonToSpawn;

    private List<DemonBase> m_spawnedDemons;
    [SerializeField] float m_spawnTimer = 6f;
    [SerializeField] int m_maxSpawnedDemons;
    float maxRange;

    private float m_timer;

    public float MaxRange { get => maxRange; set => maxRange = value; }

    private void Start()
    {
        m_timer = m_spawnTimer;
        m_spawnedDemons = new List<DemonBase>();
    }

    // Update is called once per frame
    void Update()
    {
        if (ActiveDemons() < m_maxSpawnedDemons)
        {
            m_timer -= Time.deltaTime;
            if (m_timer <= 0)
            {
                m_spawnedDemons.Add(Instantiate(m_demonToSpawn, transform.position, Quaternion.identity));
                if(maxRange != 0)
                {
                    m_spawnedDemons[m_spawnedDemons.Count - 1].MaximumPossessionRange = maxRange;
                }

                m_timer = m_spawnTimer;
            }
        }
    }

    private int ActiveDemons()
    {
        int d = 0;
        for (int i = 0; i < m_spawnedDemons.Count; i++)
        {
            if(m_spawnedDemons[i] != null)
            {
                d++;
            }
        }
        return d;
    }
}
