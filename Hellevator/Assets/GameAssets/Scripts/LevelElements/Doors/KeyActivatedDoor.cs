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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>())
        {
            CheckOpenDoor();
        }
    }

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt(key.ToString()) == 1 /*|| LevelManager.Instance.HasKitchenKey*/)
        {
            OpenDoor();
        }
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

    public void OpenDoor()
    {
        m_doorClosed.gameObject.SetActive(false);
        m_doorOpen.gameObject.SetActive(true);
        if(m_associatedMapID != null && m_associatedMapID != "")
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
