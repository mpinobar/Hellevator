using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    SpriteRenderer m_spr;
    MaterialPropertyBlock m_mpblock;
    [SerializeField] float m_delayToStartDissolve = 1.5f;
    [SerializeField] float m_dissolveSpeed = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        m_mpblock = new MaterialPropertyBlock();
        m_spr = GetComponent<SpriteRenderer>();
    }

    public void StartDissolve()
    {
        StartCoroutine(DissolveVisual());
    }
    IEnumerator DissolveVisual()
    {
        yield return new WaitForSeconds(m_delayToStartDissolve);
        string percentName = "_DissolvePercent";
        float percent = 1;
        while (percent > 0)
        {
            percent -= Time.deltaTime * m_dissolveSpeed;
            m_mpblock.SetFloat(percentName, percent);
            m_spr.SetPropertyBlock(m_mpblock);
            yield return null;
        }        
    }

    public void StartReverseDissolve()
    {
        StartCoroutine(ReverseDissolveVisual());
    }

    IEnumerator ReverseDissolveVisual()
    {
        yield return new WaitForSeconds(m_delayToStartDissolve);
        string percentName = "_DissolvePercent";
        float percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime * m_dissolveSpeed;
            percent = Mathf.Clamp01(percent);
            m_mpblock.SetFloat(percentName, percent);
            m_spr.SetPropertyBlock(m_mpblock);
            yield return null;
        }
    }

}
