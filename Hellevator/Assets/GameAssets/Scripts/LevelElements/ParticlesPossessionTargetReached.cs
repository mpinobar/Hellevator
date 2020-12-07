using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlesPossessionTargetReached : MonoBehaviour
{

    [SerializeField] AnimationCurve m_animCurveScale;
    [SerializeField] AnimationCurve m_animCurveAlpha;
    [SerializeField] float m_speed = 2f;
    [SerializeField] bool m_destroysOnCompleted = true;
    [SerializeField] float m_delay = 0f;
    Transform m_targetToFollow;
    float m_initialScale;
    SpriteRenderer spr;
    // Start is called before the first frame update
    IEnumerator Start()
    {
         spr = GetComponent<SpriteRenderer>();
        spr.enabled = false;
        yield return new WaitForSeconds(m_delay);
        spr.enabled = true;

        StartCoroutine(Animate());
    }


    private IEnumerator Animate()
    {
        m_initialScale = transform.localScale.x;
        Vector3 scale;

        float time = 0;
        
        Color color = spr.color;
        
        while (time < m_animCurveScale.length)
        {
            color.a = m_animCurveAlpha.Evaluate(time);
            spr.color = color;
            if (m_targetToFollow)
                transform.position = m_targetToFollow.position;
            scale = Vector3.one * m_initialScale * m_animCurveScale.Evaluate(time);
            transform.localScale = scale;
            time += Time.unscaledDeltaTime * m_speed;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        if (m_destroysOnCompleted)
            Destroy(gameObject);
        else
        {
            spr.enabled = false;
        }
    }

    //private IEnumerator AnimateReverse()
    //{
    //    Vector3 scale;
    //    m_initialScale = transform.localScale.x;
    //    float time = 0;
    //    SpriteRenderer spr = GetComponent<SpriteRenderer>();
    //    Color color = spr.color;
    //    while (time < m_animCurveScale.length)
    //    {
    //        color.a = m_animCurveAlpha.Evaluate(time);
    //        spr.color = color;
    //        //transform.position = m_targetToFollow.position;
    //        scale = Vector3.one * m_initialScale * m_animCurveScale.Evaluate(m_animCurveScale.length - time);
    //        transform.localScale = scale;
    //        time += Time.deltaTime * m_speed;
    //        yield return null;
    //    }
    //    Destroy(gameObject);
    //}

    public void SetTarget(Transform target)
    {
        m_targetToFollow = target;
    }
}
