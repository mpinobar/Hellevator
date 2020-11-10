using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DissolvingPit : MonoBehaviour
{

    [SerializeField] AudioClip m_acidClip;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase cmpDemon = collision.GetComponentInParent<DemonBase>();
        if (cmpDemon != null)
        {
            MusicManager.Instance.PlayAudioSFX(m_acidClip, false, 0.75f);
            //the torso has entered the pit while being a ragdoll
            if (collision == cmpDemon.LimbsColliders[0])
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
