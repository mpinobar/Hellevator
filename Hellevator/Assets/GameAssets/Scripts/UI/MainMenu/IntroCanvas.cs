using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public partial class IntroCanvas : MonoBehaviour
{
    [SerializeField] Image m_fade;
    
    [SerializeField] GameObject m_puertaIzquierda;
    [SerializeField] GameObject m_puertaDerecha;

    bool m_fadingIn;
    [SerializeField] GameObject m_canvasMenu;
    float m_fadeTime = 3;
    float m_tmp;
    public static Action OnBegin;
    [SerializeField] float m_delayToActivateMouseParallax;

    public static event Action ElevatorCalled;
    

    private void Awake()
    {
        m_canvasMenu.SetActive(false);
        PlayerPrefs.DeleteAll();
        
        //Action.RemoveAll(OnBegin, OnBegin);
        OnBegin = null;       
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        
        m_fadingIn = true;
        m_tmp = m_fadeTime;
        //if (!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        //{
        //    SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
        //}
    }


    // Update is called once per frame
    private void Update()
    {
        if (Input.anyKeyDown)
        {
            OnBegin?.Invoke();
        }

        if (m_fadingIn)
        {
            //fadeIn 
            m_tmp -= Time.deltaTime;
            Color b = m_fade.color;
            b.a = (m_tmp / (m_fadeTime * 0.5f));
            m_fade.color = b;
            
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
        //m_puertaIzquierda.GetComponent<AnimacionPuerta>().Started = true;
        //m_puertaDerecha.GetComponent<AnimacionPuerta>().Started = true;
        GetComponentInChildren<BlinkingText>().gameObject.SetActive(false);
        ElevatorCalled?.Invoke();
    }
    
}
