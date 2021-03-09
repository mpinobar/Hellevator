using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultHead : MonoBehaviour
{
    Rigidbody2D m_RGB;
    Collider2D m_collider;
    BasicZombie m_demon;
    public bool m_active;
    // Start is called before the first frame update
    void Awake()
    {
        m_demon = GetComponentInParent<BasicZombie>();
        m_RGB = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_active = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_active)
        {
            m_demon.Torso.parent = null;
            m_demon.transform.position = transform.position;
            m_demon.Torso = transform;
            PossessionManager.Instance.StartDeathChoice(m_demon.transform);
            m_collider.enabled = false;
            m_active = false;
            Destroy(this);
        }
    }
}
