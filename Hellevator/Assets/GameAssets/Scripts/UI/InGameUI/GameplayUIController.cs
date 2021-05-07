using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{

    [SerializeField] GameObject m_multiplePossessionText;
    [SerializeField] Transform possessionCircle;
    float timeOffset = 0;
    private void Start()
    {
        HideDecisionTimeBar();
    }

    public void PossessionDecisionTime(float time)
    {
        StartCoroutine(DecisionTime(time));
    }

    IEnumerator DecisionTime(float time)
    {
        float t = time;
        m_multiplePossessionText.SetActive(true);
        possessionCircle = PossessionManager.Instance.ControlledDemon.m_possessionCircle;
        possessionCircle.gameObject.SetActive(true);
        possessionCircle.localScale = Vector3.one;
        //m_decisionTimeBar.transform.parent.gameObject.SetActive(true);
        while (t > 0)
        {
            //timeOffset += Time.unscaledDeltaTime*3;
            //m_decisionTimeBar.material.SetFloat("_xOffset", timeOffset);
            possessionCircle.localScale = Vector3.one * (t / time);
            t -= Time.unscaledDeltaTime;
            //m_decisionTimeBar.fillAmount = t / time;
            yield return null;
        }
        HideDecisionTimeBar();
    }

    internal void HideDecisionTimeBar()
    {
        if (possessionCircle)
            possessionCircle.gameObject.SetActive(false);
        m_multiplePossessionText.SetActive(false);
        //m_decisionTimeBar.transform.parent.gameObject.SetActive(false);        
    }
}
