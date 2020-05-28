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
    }


    public void ChangeValue()
    {
        MusicManager.Instance.SfxVolume = m_slider.value;
    }
}
