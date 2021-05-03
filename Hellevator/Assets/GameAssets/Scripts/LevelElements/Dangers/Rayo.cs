using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rayo : MonoBehaviour
{

    [SerializeField] float m_appearanceTime = 0.15f;
    [SerializeField] float m_fadeTime = 0.75f;
    [SerializeField] float m_blastRadius = 1;
    [SerializeField] AudioClip m_thunderClip;
    float m_alphaShaderDelta;
    float m_newAlpha = 0;
    float m_timer = 0f;
    float m_fadeDelta;

    SpriteRenderer m_spr;
    MaterialPropertyBlock m_mpb;
    Color color;
    Vector4 noiseOffset;
    bool m_appearing;
    bool active;
    private void Start()
    {
        m_spr = GetComponent<SpriteRenderer>();
        m_mpb = new MaterialPropertyBlock();
        m_spr.GetPropertyBlock(m_mpb);
        m_mpb.SetFloat("_alphaHeight", 0);
        m_appearing = true;
        m_alphaShaderDelta = 1 / m_appearanceTime;
        m_fadeDelta = 1 / m_fadeTime;
        color = m_spr.color;
        noiseOffset = new Vector4(Random.value, Random.value, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (m_appearing)
        {
            m_timer += Time.deltaTime;
            m_newAlpha += m_alphaShaderDelta * Time.deltaTime;
            m_mpb.SetFloat("_alphaHeight", m_newAlpha);
            m_spr.SetPropertyBlock(m_mpb);
            if (m_timer >= m_appearanceTime)
            {
                m_appearing = false;
                m_timer = 0;
                m_mpb.SetFloat("_alphaHeight", 1);
                m_mpb.SetVector("_noiseSpeed", new Vector4(0, 0, 0, 0));
                m_spr.SetPropertyBlock(m_mpb);
                DealDamage();
            }
        }
        else
        {
            color.a -= m_fadeDelta * Time.deltaTime;
            m_spr.color = color;
        }
    }

    private void DealDamage()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.GetChild(0).position,m_blastRadius);
        CameraManager.Instance.CameraShakeMedium();
        //Debug.DrawRay(transform.GetChild(0).position, Vector3.right, Color.green, 2);
        //Debug.DrawRay(transform.GetChild(0).position, Vector3.up, Color.green, 2);
        //Debug.DrawRay(transform.GetChild(0).position, -Vector3.right, Color.green, 2);
        //Debug.DrawRay(transform.GetChild(0).position, -Vector3.up, Color.green, 2);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out DemonBase character))
            {
                if (character.IsControlledByPlayer)
                {
                    character.Die(true);
                }
            }
        }
    }

    private void OnEnable()
    {
        if (m_mpb == null)
            Start();
        m_mpb.SetFloat("_alphaHeight", 0);
        m_appearing = true;
        m_timer = 0;
        m_newAlpha = 0;
        color.a = 1;
        m_spr.color = color;
        noiseOffset += Vector4.one * Random.value;
        m_mpb.SetVector("_noiseOffset", noiseOffset);
        m_spr.SetPropertyBlock(m_mpb);
        AudioManager.Instance.PlayAudioSFX(m_thunderClip, false);
    }
}
