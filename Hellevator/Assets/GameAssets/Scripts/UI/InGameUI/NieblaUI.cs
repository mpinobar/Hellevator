using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NieblaUI : MonoBehaviour
{
    [SerializeField] float m_fogSpeed = 2f;
    SpriteRenderer m_spriteRenderer;
    MaterialPropertyBlock m_propertyBlock;
    private void OnEnable()
    {
        StopAllCoroutines();
        StartCoroutine(Niebla());
    }

    IEnumerator Niebla()
    {
        if (m_propertyBlock == null)
            m_propertyBlock = new MaterialPropertyBlock();
        if (!m_spriteRenderer)
            m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_spriteRenderer.GetPropertyBlock(m_propertyBlock);
        float f = 0f;
        while (true)
        {
            f += Time.unscaledDeltaTime * m_fogSpeed;
            m_propertyBlock.SetFloat("_offset", f);
            m_spriteRenderer.SetPropertyBlock(m_propertyBlock);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
    }
}
