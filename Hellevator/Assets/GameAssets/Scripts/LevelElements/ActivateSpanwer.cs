using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateSpanwer : MonoBehaviour
{
    [SerializeField] Spawner m_spawnerToActivate;
    [SerializeField] float newMaxRangePossesion;

    public float NewMaxRangePossesion { get => newMaxRangePossesion; set => newMaxRangePossesion = value; }

    private void Start()
    {
        m_spawnerToActivate.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.root.GetComponent<DemonBase>().MaximumPossessionRange = newMaxRangePossesion;

        m_spawnerToActivate.enabled = true;
        m_spawnerToActivate.MaxRange = newMaxRangePossesion;
    }
}
