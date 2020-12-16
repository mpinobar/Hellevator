using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUIController : MonoBehaviour
{

    [SerializeField] Image m_decisionTimeBar;


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
        m_decisionTimeBar.transform.parent.gameObject.SetActive(true);
        while (t > 0)
        {
            t -= Time.unscaledDeltaTime;
            m_decisionTimeBar.fillAmount = t / time;
            yield return null;
        }
        HideDecisionTimeBar();
    }

    internal void HideDecisionTimeBar()
    {
        m_decisionTimeBar.transform.parent.gameObject.SetActive(false);        
    }
}
