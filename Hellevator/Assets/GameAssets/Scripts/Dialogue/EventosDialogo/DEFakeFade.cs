using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DEFakeFade : DialogEvent
{
    [SerializeField] private Image m_image;
    [SerializeField] private float m_fakeFadeSpeed = 0.2f;
    [SerializeField] private float m_maxOpacity = 0f;
    private bool m_faking = false;

    [SerializeField] private List<DialogEvent> m_eventos;

    public override void ActivateEvent()
    {
        if (!m_faking)
        {
            m_faking = true;
        }
    }

    private void Update()
    {
        if (m_faking)
        {
            PossessionManager.Instance.ControlledDemon.CanMove = false;
            float a = m_image.color.a + Time.deltaTime * m_fakeFadeSpeed;
            a = Mathf.Clamp01(a);
            m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, a);

            if(a >= m_maxOpacity)
            {
                m_faking = false;
                m_image.color = new Color(m_image.color.r, m_image.color.g, m_image.color.b, 0);
                for (int i = 0; i < m_eventos.Count; i++)
                {
                    m_eventos[i].ActivateEvent();
                }
            }
        }
    }
}

