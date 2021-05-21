using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCredits : MonoBehaviour
{
    [SerializeField] Transform positionToSetAfterEndCredits;
    [SerializeField] GameObject m_credits;
    public static bool isInCredits;
    public void ShowEndCredits()
    {
        isInCredits = true;
        m_credits.SetActive(true);
        ((BasicZombie)PossessionManager.Instance.ControlledDemon).OnJumped += HideEndCredits;
    }
   
    public void HideEndCredits()
    {
        isInCredits = false;
        m_credits.gameObject.SetActive(false);
        PossessionManager.Instance.ControlledDemon.MyRgb.isKinematic = false;
        ((BasicZombie)PossessionManager.Instance.ControlledDemon).OnJumped -= HideEndCredits;
        PossessionManager.Instance.ControlledDemon.transform.position = positionToSetAfterEndCredits.position;
    }

    private void OnDisable()
    {
        ((BasicZombie)PossessionManager.Instance.ControlledDemon).OnJumped -= HideEndCredits;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
            {
                ShowEndCredits();
                ((BasicZombie)PossessionManager.Instance.ControlledDemon).ResetJumps();
                character.MyRgb.isKinematic = true;
                character.MyRgb.velocity = Vector2.zero;                
            }
        }
    }
}
