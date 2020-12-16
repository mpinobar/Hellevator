using UnityEngine;
using TMPro;



public class BlinkingText : MonoBehaviour
{
    TextMeshProUGUI m_tmpro;
    bool textDecreasingAlpha;

    private void Start()
    {
        m_tmpro = GetComponent<TextMeshProUGUI>();
        textDecreasingAlpha = false;
        Color c = m_tmpro.color;
        c.a = 0;
        m_tmpro.color = c;
    }
    private void TextFadeInAndOut()
    {
        Color c = m_tmpro.color;
        if (textDecreasingAlpha)
        {
            c.a -= Time.deltaTime;
            m_tmpro.color = c;
            if (c.a <= 0)
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

    private void Update()
    {
        TextFadeInAndOut();
    }
}



