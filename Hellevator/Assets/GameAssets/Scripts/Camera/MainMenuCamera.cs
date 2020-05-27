﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuCamera : MonoBehaviour
{
    private MenuCameraState m_state = MenuCameraState.Default;
    [SerializeField] private GameObject m_default;
    [SerializeField] private GameObject m_options;
    [SerializeField] private GameObject m_levels;
    bool m_hasChanged;
    bool m_fadingIn;
    [SerializeField] float m_fadeTime;
    private float m_tmp;
        
    // Update is called once per frame
    void Update()
    {
        if (!m_hasChanged)
        {
            if (m_fadingIn)
            {
                //fadeIn 
                m_tmp -= Time.deltaTime;
                if (m_tmp <= 0)
                {
                    SetMenuActive();                    
                    m_tmp = m_fadeTime;
                    m_fadingIn = false;
                }
            }
            else
            {
                //fade out
                m_tmp -= Time.deltaTime;
                if (m_tmp <= 0)
                {
                    m_tmp = m_fadeTime;
                    m_fadingIn = true;
                    m_hasChanged = true;
                }
            }
        }
    }


    public void ChangeState(MenuCameraState state)
    {
        m_state = state;
        m_hasChanged = false;
    }


    private void SetMenuActive()
    {
        switch(m_state)
        {
            case MenuCameraState.Default:
                m_default.SetActive(true);
                m_options.SetActive(false);
                m_levels.SetActive(false);
                break;
            case MenuCameraState.LevelSelection:
                m_default.SetActive(false);
                m_options.SetActive(false);
                m_levels.SetActive(true);
                break;
            case MenuCameraState.Options:
                m_default.SetActive(false);
                m_options.SetActive(true);
                m_levels.SetActive(false);
                break;
            default:
                break;
        }
    }
}