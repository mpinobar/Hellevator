﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversablePlatform : MonoBehaviour
{
    //Collider2D m_traversableCollider;
    Collider2D m_myCollider;
    [SerializeField] GameObject m_canvasShowTutorialTraversePlatform;
    bool m_showingTutorial;

    private void Start()
    {
        m_myCollider = GetComponent<Collider2D>();
        InputManager.Instance.OnInteract += EndTutorial;        
    }

    private void EndTutorial()
    {
        if (m_showingTutorial)
        {
            m_canvasShowTutorialTraversePlatform.SetActive(false);
            m_showingTutorial = false;
            PossessionManager.Instance.ControlledDemon.CanMove = true;
            InputManager.Instance.IsInInteactionTrigger = false;
            //Destroy(this.gameObject);
        }
    }

    public void Traverse()
    {        
        m_myCollider.enabled = false;
        StartCoroutine(ReactivateCollider());
    }

    IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(0.35f);
        m_myCollider.enabled = true;
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if(!PlayerPrefs.HasKey("TutorialTraverse") || PlayerPrefs.GetInt("TutorialTraverse") != 1)
        {
            if (collision.transform.GetComponentInParent<DemonBase>() == PossessionManager.Instance.ControlledDemon && collision.transform.position.y > transform.position.y)
            {
                if (!PossessionManager.Instance.MultiplePossessionIsUnlocked)
                {
                    InputManager.Instance.IsInInteactionTrigger = true;
                    m_canvasShowTutorialTraversePlatform.SetActive(true);
                    m_showingTutorial = true;
                    PossessionManager.Instance.ControlledDemon.CanMove = false;
                    PlayerPrefs.SetInt("TutorialTraverse", 1);
                }
            }
        }
        
    }

}
