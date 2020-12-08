using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapDoor : MonoBehaviour
{
    [SerializeField] Key m_associatedKey;
    [SerializeField] string m_ID;
    Image m_imgCmp;
    void OnEnable()
    {
        if (!m_imgCmp)
        {
            m_imgCmp = GetComponent<Image>();
        }
        if (PlayerPrefs.HasKey(m_associatedKey.ToString()) && PlayerPrefs.GetInt(m_associatedKey.ToString()) == 1)
        {
            if(PlayerPrefs.GetInt(m_ID) == 1)
            {
                SetUnlocked();
            }
            else
            {
                SetAvailableButNotUnlocked();
            }
        }else
        {
            m_imgCmp.color = Color.blue;
        }
    }   
    
    public void SetAvailableButNotUnlocked()
    {
        m_imgCmp.color = Color.red;
    }

    public void SetUnlocked()
    {
        m_imgCmp.color = Color.yellow;
    }
}
