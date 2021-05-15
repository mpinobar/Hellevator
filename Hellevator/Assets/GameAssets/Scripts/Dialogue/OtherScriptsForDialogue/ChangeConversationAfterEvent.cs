using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConversationAfterEvent : MonoBehaviour
{
    [SerializeField] private GameObject m_dialogoBeforeKey = null;
    [SerializeField] private GameObject m_dialogoAfterKey = null;
    [SerializeField] Key key;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayerPrefs.GetInt(key.ToString()) == 1)
        {
            m_dialogoBeforeKey.SetActive(false);
            m_dialogoAfterKey.SetActive(true);
        }

        Destroy(this.gameObject);
    }
}
