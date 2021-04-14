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

    [SerializeField] float distanceToStop = 2;
    [SerializeField] LayerMask raycastLayer = 1<<0;
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
    [SerializeField] float distanceToSet = 2f;
    private void FixedUpdate()
    {
        if (Speed != 0)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position,transform.up,Mathf.Infinity,raycastLayer);
            //Debug.LogError("hit " + hit.transform.name + "distance "+hit.distance);
            if (hit.distance < distanceToStop)
            {
                transform.position = (Vector3) hit.point - transform.up * distanceToSet;
                m_rgb.isKinematic = true;
                Speed = 0;
            }
        }
    }
    public void DeactivateProjectile()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.TryGetComponent(out HorizontalTransport transport))
        //{
        //    //Debug.LogError("Colliding with platform");
        //    //Speed = 0;
        //    //m_rgb.isKinematic = true;
        //    //m_dragging = true;
        //    //m_dragVelocity = Vector2.right * transport.m_speed * transport.dir;
        //}
        //else
        //{
        //    m_rgb.isKinematic = true;
        //    Speed = 0;
        //    GetComponent<Collider2D>().enabled = false;
        //}
        if (collision.TryGetComponent(out DemonBase dem))
        {
            dem.Die(true);
            transform.parent = dem.DemonMaskSprite.transform.parent;
        }
        m_rgb.isKinematic = true;
        Speed = 0;
        GetComponent<Collider2D>().enabled = false;
        EndBossKnife();
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

    private void EndBossKnife()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(transform.position,1);
        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].TryGetComponent(out DemonBase dem))
            {
                dem.IsInDanger = false;

                return;
            }else if(cols[i].TryGetComponent(out RagdollLogicalCollider rlc))
            {
                rlc.ParentDemon.IsInDanger = false;
                return;
            }
        }
    }
}
