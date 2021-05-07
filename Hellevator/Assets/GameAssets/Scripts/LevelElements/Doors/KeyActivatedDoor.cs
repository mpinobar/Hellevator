using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActivatedDoor : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] GameObject m_doorClosed;
    [SerializeField] GameObject m_doorOpen;
    [SerializeField] GameObject m_keyNeededText;
    [SerializeField] string m_associatedMapID;
    [SerializeField] AudioClip m_lockHitClip;
    [SerializeField] AudioClip m_lockOpenClip;
    [SerializeField] Animator animationOpenDoor;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>())
        {
            CheckOpenDoor();
        }
    }

    private void OnEnable()
    {
        //if (PlayerPrefs.GetInt(key.ToString()) == 1 /*|| LevelManager.Instance.HasKitchenKey*/)
        //{
        //    OpenDoor();
        //}
    }

    private void CheckOpenDoor()
    {
        if (PlayerPrefs.GetInt(key.ToString()) == 1 /*|| LevelManager.Instance.HasKitchenKey*/)
        {
            OpenDoor();
        }
        else
        {
            if (m_keyNeededText)
                ShowKeyNeeded();
        }
    }

    private void ShowKeyNeeded()
    {
        m_keyNeededText.SetActive(true);
    }
    [SerializeField] float delayToPlayEffects = 1.1f;
    public IEnumerator DelayPlaySoundAndShake()
    {
        yield return new WaitForSeconds(delayToPlayEffects);
        AudioManager.Instance.PlayAudioSFX(m_lockHitClip, false);
        
        CameraManager.Instance.CameraShakeLight();
        m_doorClosed.GetComponent<Collider2D>().enabled = false;
    }

    public void OpenDoor()
    {
        //m_doorClosed.GetComponent<Collider2D>().enabled = false;
        AudioManager.Instance.PlayAudioSFX(m_lockOpenClip, false); 
        animationOpenDoor.SetTrigger("Open");
        StartCoroutine(DelayPlaySoundAndShake());
        if (m_associatedMapID != null && m_associatedMapID != "")
        {
            PlayerPrefs.SetInt(m_associatedMapID, 1);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_keyNeededText)
        {
            if (collision.GetComponentInParent<DemonBase>())
            {
                HideKeyNeeded();
            }
        }

    }

    private void HideKeyNeeded()
    {
        m_keyNeededText.SetActive(false);
    }
}
