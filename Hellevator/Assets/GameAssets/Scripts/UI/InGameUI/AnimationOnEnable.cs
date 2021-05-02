using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationOnEnable : MonoBehaviour
{
    [SerializeField] Vector2 m_startingOffset;
    [SerializeField] float m_transitionSpeed = 2.5f;
    Vector2 m_endPosition;
    Vector2 m_startingPosition;

    public bool isOut;

    private void Awake()
    {        
        m_endPosition = transform.localPosition;
        m_startingPosition = m_endPosition + m_startingOffset;
    }
    public void OnEnable()
    {
        //Debug.LogError("Enabling hotel background");
        transform.localPosition = m_startingPosition;
        StartAnimate();
    }

    private void StartAnimate()
    {
        StartCoroutine(Animate());
    }
    IEnumerator Animate()
    {
        isOut = false;
        while (Vector2.Distance(transform.localPosition, m_endPosition) > 0.1f)
        {
            transform.localPosition = Vector2.Lerp(transform.localPosition, m_endPosition, Time.unscaledDeltaTime * m_transitionSpeed);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        //transform.position = m_endPosition;
    }

    public void Stop()
    {
        isOut = true;
        StopAllCoroutines();
    }
}
