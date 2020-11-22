using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IntroCanvas : MonoBehaviour
{
    [SerializeField] Image m_fade;
    [SerializeField] TextMeshProUGUI m_tmpro;
    [SerializeField] GameObject m_puertaIzquierda;
    [SerializeField] GameObject m_puertaDerecha;

    bool m_fadingIn;
    [SerializeField] GameObject m_canvasMenu;
    float m_fadeTime = 3;
    float m_tmp;

    bool textDecreasingAlpha;
    private void Awake()
    {
        m_canvasMenu.SetActive(false);
        textDecreasingAlpha = false;
        Color c = m_tmpro.color;
        c.a = 0;
        m_tmpro.color = c;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        m_tmpro.gameObject.SetActive(false);
        m_fadingIn = true;
        m_tmp = m_fadeTime;
        if (!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        {
            SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_fadingIn)
        {
            //fadeIn 
            m_tmp -= Time.deltaTime;
            Color b = m_fade.color;
            b.a = (m_tmp / (m_fadeTime * 0.5f));
            m_fade.color = b;
            if (m_tmp <= 0)
            {
                if (!m_tmpro.gameObject.activeSelf)
                {
                    m_tmpro.gameObject.SetActive(true);
                }
                TextFadeInAndOut();    
            }
        }
        else
        {
            //fade out
            m_tmp -= Time.deltaTime;
            Color b = m_fade.color;
            b.a = 1- (m_tmp / (m_fadeTime * 0.5f));
            m_fade.color = b;
            if (m_tmp <= 0)
            {
                m_tmp = m_fadeTime * 0.5f;
                m_fadingIn = true;
                m_canvasMenu.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }

    public void CallElevator()
    {
        m_tmp = m_fadeTime * 0.5f;
        m_fadingIn = false;
        m_puertaIzquierda.GetComponent<AnimacionPuerta>().Started = true;
        m_puertaDerecha.GetComponent<AnimacionPuerta>().Started = true;
    }

    private void TextFadeInAndOut()
    {
        Color c = m_tmpro.color;
        if (textDecreasingAlpha)
        {
            c.a -= Time.deltaTime;
            m_tmpro.color = c;
            if(c.a <= 0)
            {
                textDecreasingAlpha = false;
            }
        }
        else
        {
            c.a += Time.deltaTime;
            m_tmpro.color = c;
            if (c.a >= 1)
            {
                textDecreasingAlpha = true;
            }
        }
    }
}
