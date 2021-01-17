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
        Debug.LogError("Petrifying");
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
        platform.GetComponent<DestructiblePlatform>().TurnsKinematicOnSpikesEnter = true;
        platform.GetComponent<DestructiblePlatform>().TurnsKinematicOnCollisionEnter = m_platformTurnsKinematicOnCollisionEnter;
        platform.GetComponent<DestructiblePlatform>().WillReappear = false;

        //PossessionManager.Instance.RemoveDemonPossession(transform);

        gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    public void SetCantPetrify()
    {
        m_cantPetrify = true;
    }
}
