using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glow : MonoBehaviour
{
    [SerializeField]
    [ColorUsage(true, true)]
    Color m_currentColor;
    SpriteRenderer m_spriteRenderer;
    bool m_glowing;
    [SerializeField] float m_changeSpeed = 1f;
    [SerializeField] float m_variance = 0.25f;
    [SerializeField] float m_initialIntensity = 1.5f;
    float m_currentIntensity;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_currentColor = m_spriteRenderer.color;

        m_currentIntensity = m_initialIntensity;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_glowing)
        {
            m_currentIntensity += Time.deltaTime * m_changeSpeed;
            if (m_currentIntensity >= m_initialIntensity + m_variance)
            {
                m_glowing = false;
            }
            //m_currentColor = new Vector4(m_currentColor.r, m_currentColor.g, m_currentColor.b, m_currentIntensity);
        }
        else
        {
            m_currentIntensity -= Time.deltaTime * m_changeSpeed;
            if (m_currentIntensity <= m_initialIntensity - m_variance)
            {
                m_glowing = true;
            }
        }
        m_spriteRenderer.material.SetColor("_MainColor", m_currentColor * m_currentIntensity);
        //m_currentColor = new Vector4(m_currentColor.r, m_currentColor.g, m_currentColor.b, m_currentIntensity);
    }
}
