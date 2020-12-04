using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseParallax : MonoBehaviour
{
    //[SerializeField] float m_parallaxMultiplier = 1f;
    [SerializeField] float m_verticalParallaxMultiplier = 1f;
    [SerializeField] float m_horizontalParallaxMultiplier = 1f;
    Vector2 m_initialPosition;
    Vector2 m_offset;

    Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        m_initialPosition = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector2 mousePosition = cam.viewpo();
        //Debug.LogError(cam.ScreenToViewportPoint(Input.mousePosition));
        Vector2 offset = cam.ScreenToViewportPoint(Input.mousePosition);
        offset.x *= m_horizontalParallaxMultiplier;
        offset.y *= m_verticalParallaxMultiplier;
        transform.localPosition = m_initialPosition - offset;
    }
}
