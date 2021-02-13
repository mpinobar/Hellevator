using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultHead : MonoBehaviour
{
    Rigidbody2D m_RGB;
    Collider2D m_collider;
    BasicZombie m_demon;

    // Start is called before the first frame update
    void Start()
    {
        m_demon = GetComponentInParent<BasicZombie>();
        m_RGB = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        m_demon.Torso.parent = null;
        m_demon.transform.position = transform.position;
        m_demon.Torso = transform;
        PossessionManager.Instance.StartDeathChoice(m_demon.transform);
    }
}
