using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectionCristal : MonoBehaviour
{

    Camera m_cam;
    SpriteRenderer m_spriteRenderer;


    void Start()
    {
        m_spriteRenderer = transform.parent.GetComponent<SpriteRenderer>();
        m_cam = GetComponent<Camera>();        
        GenerateRT();
    }

    private void GenerateRT()
    {
        RenderTexture rt = new RenderTexture(m_cam.targetTexture);
        m_cam.targetTexture = null;
        m_cam.targetTexture = rt;
        m_cam.targetTexture.filterMode = FilterMode.Bilinear;
        //Shader.SetGlobalTexture("_Texture", m_cam.targetTexture);
        m_spriteRenderer.material.SetTexture("Texture2D_9BB80B41", rt);
    }
}
