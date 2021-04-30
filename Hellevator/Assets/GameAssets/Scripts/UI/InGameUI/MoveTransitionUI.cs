using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTransitionUI : MonoBehaviour
{
    RectTransform m_transform;
    [SerializeField] Vector2 m_startingOffset;
    [SerializeField] float m_transitionSpeed = 2.5f;
    Vector2 m_endPosition;
    Vector2 m_startingPosition;

    //[SerializeField] MoveTransitionUI m_transformToReverse;

    private void Awake()
    {
        m_transform = GetComponent<RectTransform>();
        m_endPosition = m_transform.localPosition;
        m_startingPosition = m_endPosition + m_startingOffset;
        
    }
    private void OnEnable()
    {
        m_transform.localPosition = m_startingPosition;
        StartAnimate();
    }

    private void StartAnimate()
    {
        StartCoroutine(Animate());
    }
    IEnumerator Animate()
    {
        while (Vector2.Distance(m_transform.localPosition, m_endPosition) > 0.1f)
        {
            m_transform.localPosition = Vector2.Lerp(m_transform.localPosition, m_endPosition, Time.unscaledDeltaTime * m_transitionSpeed);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        //transform.position = m_endPosition;
    }

}
