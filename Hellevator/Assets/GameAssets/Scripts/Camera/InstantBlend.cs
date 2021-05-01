using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Events;

public class InstantBlend : MonoBehaviour
{
    public UnityEvent ontriggerEnter;
    [SerializeField] CinemachineBrain brain;
    public void InstaBlend()
    {
        ontriggerEnter?.Invoke();
        float tmp = brain.m_DefaultBlend.m_Time;
        brain.m_DefaultBlend.m_Time = 0;
        //brain.ActiveBlend.Duration = 0;
        StartCoroutine(ResetBlendDuration(tmp));
    }

    IEnumerator ResetBlendDuration(float newDur)
    {
        yield return new WaitForSeconds(0.25f);
        brain.m_DefaultBlend.m_Time = newDur;
    }
}
