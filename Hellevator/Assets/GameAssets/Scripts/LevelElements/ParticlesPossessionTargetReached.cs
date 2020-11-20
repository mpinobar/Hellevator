using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPossessionTargetReached : MonoBehaviour
{

    [SerializeField] AnimationCurve m_animCurveScale;
    [SerializeField] AnimationCurve m_animCurveAlpha;
    [SerializeField] float m_speed = 2f;
    Transform m_targetToFollow;
    float m_initialScale;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Animate());    
    }

    private IEnumerator Animate()
    {
        Vector3 scale;
        m_initialScale = transform.localScale.x;
        float time = 0;
        SpriteRenderer spr = GetComponent<SpriteRenderer>();
        Color color = spr.color;
        while (time < m_animCurveScale.length)
        {
            color.a = m_animCurveAlpha.Evaluate(time);
            spr.color = color;
            transform.position = m_targetToFollow.position;
            scale = Vector3.one * m_initialScale * m_animCurveScale.Evaluate(time);
            transform.localScale = scale;
            time += Time.deltaTime * m_speed;
            yield return null;
        }
        Destroy(gameObject);
    }
    public void SetTarget(Transform target)
    {
        m_targetToFollow = target;
    }
}
