using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectableSlider : Selectable
{
    [SerializeField] Slider m_sliderToAffect;

    [SerializeField] Text m_textToShowValue;
    [SerializeField] bool isSFX;
    private void Awake()
    {
        if (!m_sliderToAffect)
        {
            m_sliderToAffect = GetComponentInChildren<Slider>();
        }
        
    }
    private void OnEnable()
    {
        ChangeValue();
    }

    public override void NavigateRight()
    {
        //Debug.LogError("Current value" + m_sliderToAffect.value + " new value "+ (m_sliderToAffect.value + 0.05f));
        if (isSFX)
        {
            AudioManager.SfxVolume += 0.05f;
        }
        else
        {
            AudioManager.MusicVolume += 0.05f;
        }
        ChangeValue();
        
    }

    public void ChangeValue()
    {

        m_sliderToAffect.value = isSFX ? AudioManager.SfxVolume : AudioManager.MusicVolume;
        RefreshText();
        if (!isSFX)
            AudioManager.Instance.RefreshVolume();
    }

    public override void NavigateLeft()
    {
        if (isSFX)
        {
            AudioManager.SfxVolume -= 0.05f;
        }
        else
        {
            AudioManager.MusicVolume -= 0.05f;
        }
        ChangeValue();
    }

    private void RefreshText()
    {
        if (m_textToShowValue)
            m_textToShowValue.text = "" + Mathf.RoundToInt(m_sliderToAffect.value * 100);
    }
}
