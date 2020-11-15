using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingPit : MonoBehaviour
{

    [SerializeField] AudioClip m_acidClip;
    SpriteRenderer m_spriteRenderer;

    private void Start()
    {
        m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        if (cmpDemon != null)
        {
            if (m_spriteRenderer)
            {
                if(m_spriteRenderer.isVisible)
                    MusicManager.Instance.PlayAudioSFX(m_acidClip, false, 0.65f);
            }
            //the torso has entered the pit while being a ragdoll
            if (collision == cmpDemon.RagdollLogicCollider)
            {
                Destroy(cmpDemon.gameObject);
            }
            else if (collision == cmpDemon.PlayerCollider)
            {
                cmpDemon.Die(false);
            }
        }
    }
}
