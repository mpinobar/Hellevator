using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatapultHead : MonoBehaviour
{
    Rigidbody2D m_RGB;
    Collider2D m_collider;
    BasicZombie m_demon;
    public bool m_active;
    Vector3 m_lastPosition;

    public Vector3 LastPosition { get => m_lastPosition; set => m_lastPosition = value; }

    // Start is called before the first frame update
    void Awake()
    {
        m_demon = GetComponentInParent<BasicZombie>();
        m_RGB = GetComponent<Rigidbody2D>();
        m_collider = GetComponent<Collider2D>();
        m_active = false;
    }

    float airTimer;
    private void Update()
    {
        if (m_active)
        {
            
            airTimer += Time.deltaTime;
            if (airTimer >= 3)
            {
                m_active = false;
                LevelManager.Instance.RestartLevel();
            }
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (m_active)
        {
            //Debug.LogError(m_RGB.velocity);
            m_demon.Torso.parent = null;
            m_demon.transform.position = transform.position;
            m_demon.Torso = transform;
            PossessionManager.Instance.StartDeathChoice(m_demon.transform);
            m_collider.enabled = false;
            m_active = false;
            StartCoroutine(SetGravityOfhead());
        }
    }

    IEnumerator SetGravityOfhead()
    {
        yield return new WaitForSeconds(0.05f);
        m_RGB.isKinematic = false;
        m_RGB.gravityScale = 8f;
        Destroy(this);
    }
}
