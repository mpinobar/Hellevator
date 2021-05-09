using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpacityFadeoutOnEnable : MonoBehaviour
{
    Image imgCmp;
    Color initialColor;
    [SerializeField] float m_fadeTime = 0.5f;
    float m_timer;

    float rateAlphaChange;
    private void Awake()
    {
        rateAlphaChange = 1 / m_fadeTime;
        imgCmp = GetComponent<Image>();
        initialColor = imgCmp.color;
        currentColor = initialColor;
        m_timer = m_fadeTime;
    }
    Color currentColor;
    // Start is called before the first frame update
    void OnEnable()
    {
        m_timer = m_fadeTime;
        imgCmp.color = initialColor;
        currentColor = initialColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_timer > 0)
        {
            m_timer -= Time.deltaTime;
            currentColor.a -= Time.deltaTime * rateAlphaChange;
            imgCmp.color = currentColor;
        }
        else
        {

            gameObject.SetActive(false);
        }
    }
}
