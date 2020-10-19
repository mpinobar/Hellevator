using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : TemporalSingleton<Boss>
{

    private enum State
    {
        Default, SeeingPlayer
    }

    [SerializeField] GameObject m_doorToCloseUponStart;
    [SerializeField] float m_timeUntilPlayerDeath;
    float m_playerSeenDeathTimer = 0f;

    State m_currentState = State.Default;

    public override void Awake()
    {
        base.Awake();
        m_playerSeenDeathTimer = 0f;
    }


    // Update is called once per frame
    void Update()
    {
        if(m_currentState == State.SeeingPlayer)
        {
            m_playerSeenDeathTimer += Time.deltaTime;
            if(m_playerSeenDeathTimer >= m_timeUntilPlayerDeath)
            {
                KillPlayer();
            }
        }        
    }

    public void SetSeeingPlayer()
    {
        m_currentState = State.SeeingPlayer;
    }

    public void SetNotSeeingPlayer()
    {
        m_currentState = State.Default;
        m_playerSeenDeathTimer = 0f;
    }

    private void KillPlayer()
    {
        throw new NotImplementedException();
    }

    public void CloseEntrance()
    {
        //m_doorToCloseUponStart.Close();
    }
}
