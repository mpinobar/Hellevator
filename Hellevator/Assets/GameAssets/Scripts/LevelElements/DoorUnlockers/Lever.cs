using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Lever : ActivatedBase
{
    //[SerializeField] Key key;
    [SerializeField] ButtonActivatedDoor m_doorToUnlock;
    [SerializeField] float m_visualLeverSpeed = 2f;
    [SerializeField] VideoClip m_clipToPlay;
    bool m_added;
    bool m_activated;

	[SerializeField] AudioClip m_leverClip;
    [SerializeField] Transform m_rotatingObject;


    /// <summary>
    /// Visual de la palanca y de objeto linkado, play del audio y de video si tiene
    /// </summary>
	public override void Activate()
    {
        base.Activate();
        if (!m_activated)
        {
            //(PlayerPrefs.SetInt(key.ToString(), 1);
            m_doorToUnlock.Activate();
			AudioManager.Instance.PlayAudioSFX(m_leverClip, false);
            ChangeLeverVisual();
            m_activated = true;
            if (m_clipToPlay)
            {
                CameraManager.Instance.PlayVideo(m_clipToPlay);
            }
        }
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!m_added)
        {
            if (collision.GetComponentInParent<DemonBase>())
            {
                if (collision.GetComponentInParent<DemonBase>().IsControlledByPlayer)
                {
                    InputManager.Instance.IsInInteactionTrigger = true;
                    InputManager.Instance.OnInteract += Activate;
                    m_added = true;
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_added)
        {
            if (collision.GetComponentInParent<DemonBase>())
            {
                if (collision.GetComponentInParent<DemonBase>().IsControlledByPlayer)
                {
                    InputManager.Instance.IsInInteactionTrigger = false;
                    InputManager.Instance.OnInteract -= Activate;
                    m_added = false;
                }
            }
        }        
    }

    /// <summary>
    /// Empieza la animacion de rotacion de la palanca
    /// </summary>
    void ChangeLeverVisual()
    {        
        StartCoroutine(LeverVisualRotation());
    }

    /// <summary>
    /// Anima la palanca, rotandola
    /// </summary>
    /// <returns></returns>
    IEnumerator LeverVisualRotation()
    {
        //Transform visual = transform.GetChild(0).GetChild(1);
        float currentAngle = m_rotatingObject.localEulerAngles.z;
        float endAngle = -currentAngle;

        while (currentAngle > endAngle)
        {            
            currentAngle -= Time.deltaTime * m_visualLeverSpeed;
            m_rotatingObject.localEulerAngles = Vector3.forward * currentAngle;
            yield return null;
        }
    }

    /// <summary>
    /// Animacion inmediata y logica de activacion base y de objeto linkado
    /// </summary>
    public override void ActivateImmediately()
    {
        base.ActivateImmediately();
        m_activated = true;
        m_rotatingObject.localEulerAngles = -Vector3.forward * m_rotatingObject.localEulerAngles.z;
        m_doorToUnlock.ActivateImmediately();
    }
}
