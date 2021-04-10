using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodOverlay : MonoBehaviour
{
    [SerializeField] float m_timeBeforeFadeOff = 1f;

    [SerializeField] float m_minNoiseScale = 20f;
    [SerializeField] float m_maxNoiseScale = 100f;

    //MaterialPropertyBlock m_pblock;
    Image m_spr;
    const string m_bloodColorName = "_BloodColor";
    const string m_scaleName = "_NoiseScale";

    Color m_currentColor;

    Color m_initialBloodColor;
    private void Start()
    {
        m_spr = GetComponent<Image>();
       // m_pblock = new MaterialPropertyBlock();
        m_initialBloodColor = m_spr.material.GetColor(m_bloodColorName);
        m_initialBloodColor.a = 1;
        m_currentColor = m_initialBloodColor;
        m_currentColor.a = 0;
        m_spr.material.SetColor(m_bloodColorName, m_currentColor);
    }

    //private void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.B))
    //    {
    //        ShowOverlayAndFade();
    //    }
    //}

    public void ShowOverlayAndFade()
    {
        StopAllCoroutines();
        StartCoroutine(OverlayAndFade());
    }
    IEnumerator OverlayAndFade()
    {        
        float tmp = m_timeBeforeFadeOff;
        m_currentColor = m_initialBloodColor;
        m_spr.material.color = m_initialBloodColor;
        m_spr.material.SetFloat(m_scaleName, Random.Range(m_minNoiseScale, m_maxNoiseScale));
      
        while (tmp > 0)
        {
            tmp -= Time.deltaTime;
            m_currentColor.a = tmp / m_timeBeforeFadeOff;
            m_spr.material.SetColor(m_bloodColorName, m_currentColor);
            yield return null;
        }
    }
}
