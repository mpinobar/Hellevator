using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    float m_speed = 2f;

    [SerializeField] float m_timeToDeactivate = 5f;
    float m_timeToDeactivateTimer;
    Rigidbody2D m_rgb;

    bool m_countingDown = true;
    Vector2 m_dragVelocity;
    bool m_dragging;
    private void OnEnable()
    {
        if (!m_rgb)
        {
            m_rgb = GetComponent<Rigidbody2D>();
        }
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        //m_timeToDeactivateTimer = m_timeToDeactivate;
        GetComponent<Collider2D>().enabled = true;
        m_rgb.velocity = transform.up * Speed;
    }
    public float Speed
    {
        get => m_speed;
        set
        {
            m_speed = value;
            m_rgb.velocity = transform.up * Speed;
        }
    }


    private void Update()
    {
        //if (m_countingDown)
        //    m_timeToDeactivateTimer -= Time.deltaTime;
        //if (m_timeToDeactivateTimer <= 0)
        //{
        //    DeactivateProjectile();
        //}

        if (m_dragging)
        {
            m_rgb.velocity = m_dragVelocity;
        }
    }

    public void DeactivateProjectile()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() != null)
        {
            collision.GetComponentInParent<DemonBase>().Die(true);
        }
        else if(collision.TryGetComponent(out HorizontalTransport transport))
        {
            //Debug.LogError("Colliding with platform");
            m_rgb.isKinematic = true;
            m_dragging = true;
            m_dragVelocity = Vector2.right * transport.m_speed * transport.dir;
        }
        else
        {
            Speed = 0;
        }
        
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out HorizontalTransport transport))
        {
            //Debug.LogError("Leaving collision with platform");
            m_dragging = false;
            m_rgb.isKinematic = false;
            m_rgb.velocity = transform.up * Speed;
        }
    }


}
