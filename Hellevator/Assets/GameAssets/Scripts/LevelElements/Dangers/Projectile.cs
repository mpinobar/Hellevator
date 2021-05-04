using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float m_speed = 2f;

    [SerializeField] float m_timeToDeactivate = 5f;
    float m_timeToDeactivateTimer;

    bool m_destroyOnScenaryImpact = true;
    Rigidbody2D m_rgb;
    SpriteRenderer m_spriteCmp;
    bool m_countingDown = true;
    [SerializeField] bool m_isBossProjectile = false;
    [SerializeField] AudioClip m_impactClip;
    private void OnEnable()
    {
        if (!m_spriteCmp)
            m_spriteCmp = GetComponentInChildren<SpriteRenderer>();

        if (!m_rgb)
        {
            m_rgb = GetComponent<Rigidbody2D>();
        }
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        m_timeToDeactivateTimer = m_timeToDeactivate;
        GetComponent<Collider2D>().enabled = true;
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

    public bool DestroyOnScenaryImpact { get => m_destroyOnScenaryImpact; set => m_destroyOnScenaryImpact = value; }

    private void Update()
    {
        if (m_countingDown)
            m_timeToDeactivateTimer -= Time.deltaTime;
        if (m_timeToDeactivateTimer <= 0)
        {
            DeactivateProjectile();
        }
    }
    AudioSource src;
    public void DeactivateProjectile()
    {
        transform.localPosition = Vector3.zero;
        if (m_spriteCmp.isVisible)
        {
            if (src && src.isPlaying)
                src.Stop();
            src = AudioManager.Instance.PlayAudioSFX(m_impactClip, false);
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>() != null)
        {
            collision.GetComponentInParent<DemonBase>().Die(true);
        }
        if (m_destroyOnScenaryImpact)
        {
            if (m_isBossProjectile)
            {
                //es proyectil de boss
                StopBossKnifeLogic();
                transform.position -= Vector3.up * Random.value;
                if (collision.GetComponent<DemonBase>())
                {
                    transform.parent = collision.GetComponent<DemonBase>().DemonMaskSprite.transform.parent;
                    transform.GetChild(0).GetComponent<SpriteRenderer>().sortingLayerID = collision.GetComponent<DemonBase>().DemonMaskSprite.GetComponent<SpriteRenderer>().sortingLayerID;
                    if (collision.GetComponent<DemonBase>().DemonMaskSprite.transform.parent.childCount < 4)
                        m_countingDown = false;
                }


            }
            else
            {
                //es proyectil de spawner
                DeactivateProjectile();

            }
        }
        else
        {
            //Es proyectil de boss pero de los iniciales, no mata al jugador
            StopBossKnifeLogic();
        }
    }

    private void StopBossKnifeLogic()
    {
        m_rgb.velocity = Vector2.zero;
        m_rgb.isKinematic = true;
        m_rgb.angularVelocity = 0f;
        GetComponent<Collider2D>().enabled = false;
        transform.GetChild(0).GetComponent<Collider2D>().enabled = false;
    }
}
