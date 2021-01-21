﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    //[SerializeField] float m_parallaxMultiplier = 1f;
    [SerializeField] float m_verticalParallaxMultiplier = 1f;
    [SerializeField] float m_horizontalParallaxMultiplier = 1f;
    [SerializeField] float m_parallaxSpeed = 3.5f;
    Vector2 m_initialPosition;
    Vector2 m_distanceToCenter;
    [SerializeField] Vector2 m_offset;

    Camera cam;
    private bool active;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        m_initialPosition = transform.localPosition;
        IntroCanvas.OnBegin += ActivateWithDelay;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePosition = cam.viewpo();
        //Debug.LogError(cam.ScreenToViewportPoint(Input.mousePosition));
        if (active)
        {

            m_distanceToCenter = cam.ScreenToViewportPoint(Input.mousePosition);
            m_distanceToCenter.x *= m_horizontalParallaxMultiplier;
            m_distanceToCenter.y *= m_verticalParallaxMultiplier;
            transform.localPosition = Vector3.Lerp(transform.localPosition, m_initialPosition - m_distanceToCenter /*+ m_offset*/ /*+ Vector2.right*/, Time.deltaTime * m_parallaxSpeed);
        }
    }

    private void Activate()
    {
        active = true;
        
    }

    private void ActivateWithDelay()
    {
        Invoke(nameof(Activate), 5);
        IntroCanvas.OnBegin -= ActivateWithDelay;
    }
}
