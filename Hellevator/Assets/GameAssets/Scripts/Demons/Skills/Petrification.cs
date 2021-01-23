using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petrification : MonoBehaviour
{
    [SerializeField] DestructiblePlatform m_prefabToConvertInto;
    [SerializeField] bool m_usesGravity;
    [SerializeField] bool m_conservesPlayerMomentum;
    [SerializeField] float m_verticalOffsetToCreatePlatform = 1f;
	[SerializeField] bool m_platformTurnsKinematicOnCollisionEnter = true;
	[SerializeField] AudioClip m_createPetrificationClip;
    bool m_cantPetrify;
    /// <summary>
    /// Instantiates a platform and destroys the parent demon that created it
    /// </summary>
    public void Petrify()
    {
        if (m_cantPetrify)
        {
            return;
        }
        //Debug.LogError("Petrifying");
        TurnKinematic();
        DisableColliders();
        Rigidbody2D platform = Instantiate(m_prefabToConvertInto, GetComponent<DemonBase>().Torso.position + Vector3.up*m_verticalOffsetToCreatePlatform, Quaternion.identity, transform).GetComponent<Rigidbody2D>();
        platform.transform.parent = null;
		AudioManager.Instance.PlayAudioSFX(m_createPetrificationClip, false);

        if (!m_usesGravity)
        {
            platform.isKinematic = true;
        }
        else
        {
            
            if (m_conservesPlayerMomentum)
            {
                platform.velocity = transform.root.GetComponent<Rigidbody2D>().velocity;
            }
            platform.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        DestructiblePlatform platformComponent = platform.GetComponent<DestructiblePlatform>();
        platformComponent.TurnsKinematicOnSpikesEnter = true;
        platformComponent.TurnsKinematicOnCollisionEnter = m_platformTurnsKinematicOnCollisionEnter;
        platformComponent.WillReappear = false;

        platformComponent.GetComponentInChildren<Dissolve>().StartReverseDissolve();
        //PossessionManager.Instance.RemoveDemonPossession(transform);
        GetComponent<DemonBase>().IsPossessionBlocked = true;
        Dissolve[] dissolves = GetComponentsInChildren<Dissolve>();

        for (int i = 0; i < dissolves.Length; i++)
        {
            dissolves[i].StartDissolve();
        }
        
        Destroy(gameObject, 1);

        //gameObject.SetActive(false);

    }

    private void TurnKinematic()
    {
        Rigidbody2D [] rgbs = GetComponent<DemonBase>().LimbsRbds;
        for (int i = 0; i < rgbs.Length; i++)
        {
            rgbs[i].isKinematic = true;
        }
    }

    private void DisableColliders()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<DemonBase>().RagdollLogicCollider.gameObject.SetActive(false);
        Collider2D[] colliders = GetComponent<DemonBase>().LimbsColliders;
        for (int i = 0; i < colliders.Length; i++)
        {
            colliders[i].enabled = false;
        }
    }

    public void SetCantPetrify()
    {
        m_cantPetrify = true;
    }


}
