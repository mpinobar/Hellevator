using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSlider : MonoBehaviour
{

    Slider m_slider;

    private void Awake()
    {
        m_slider = GetComponent<Slider>();
        m_slider.value = AudioManager.MusicVolume;
    }
       
    public void ChangeValue()
    {
        AudioManager.MusicVolume = m_slider.value;
        AudioManager.Instance.ChangeBGMVolume(m_slider.value);
        Debug.LogError("Grabbed value " + m_slider.value);
    }
}
