using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SFXSlider : MonoBehaviour
{
    Slider m_slider;

    private void Awake()
    {
        m_slider = GetComponent<Slider>();
        m_slider.value = AudioManager.SfxVolume;
    }


    public void ChangeValue()
    {
        AudioManager.SfxVolume = m_slider.value;
    }
}
