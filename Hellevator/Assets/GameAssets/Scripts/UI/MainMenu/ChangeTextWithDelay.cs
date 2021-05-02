using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ChangeTextWithDelay : MonoBehaviour
{
    TextMeshProUGUI m_tmpro;
    [SerializeField] string m_newText;
    [SerializeField] float m_delay;


    // Start is called before the first frame update
    void Start()
    {
        m_tmpro = GetComponent<TextMeshProUGUI>();
        IntroCanvas.OnBegin += DelayChangeText;    
    }
    private void DelayChangeText()
    {
        m_tmpro.text = "";
        Invoke(nameof(ChangeText), m_delay);
    }
    private void ChangeText()
    {
        m_tmpro.text = m_newText;
        IntroCanvas.OnBegin -= DelayChangeText;
    }
}
