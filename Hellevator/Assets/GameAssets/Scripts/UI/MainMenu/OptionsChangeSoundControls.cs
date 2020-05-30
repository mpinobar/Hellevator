using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class OptionsChangeSoundControls : MonoBehaviour
{
    TextMeshProUGUI m_tmpro;    
    [SerializeField] GameObject m_soundsGo;
    [SerializeField] GameObject m_controlsGo;

    bool m_soundsActive;


    private void Start()
    {
        m_tmpro = GetComponentInChildren<TextMeshProUGUI>();
        m_soundsGo.SetActive(false);
        m_controlsGo.SetActive(true);
        m_soundsActive = false;
        m_tmpro.text = "Sound";
    }
    public void Swap()
    {
        if (m_soundsActive)
        {
            m_soundsGo.SetActive(false);
            m_controlsGo.SetActive(true);
            m_soundsActive = false;
            m_tmpro.text = "Sound";
        }
        else
        {
            m_soundsGo.SetActive(true);
            m_controlsGo.SetActive(false);
            m_soundsActive = true;
            m_tmpro.text = "Controls";
        }
    }
}
