using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BeginAnimation : MonoBehaviour
{
    Animator m_animator;

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_animator.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        IntroCanvas.OnBegin += Animate;
    }

    public void Animate()
    {
        //Debug.LogError("Animating GO " + name);
        m_animator.enabled = true;
        IntroCanvas.OnBegin -= Animate;
    }
 
}
