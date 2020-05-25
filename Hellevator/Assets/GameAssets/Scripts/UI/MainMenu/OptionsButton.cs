using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OptionsButton : MonoBehaviour
{
    [SerializeField] GameObject m_optionsPanel;
    bool m_optionsPanelActive;

    public void OnOptionsPressed()
    {
        ShowOptionsPanel();
    }

    private void ShowOptionsPanel()
    {
        m_optionsPanel.SetActive(true);
    }

    private void Start()
    {
        HideOptionsPanel();
    }

    private void HideOptionsPanel()
    {
        m_optionsPanel.SetActive(false);
    }
}
