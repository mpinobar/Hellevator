﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FadeManager : TemporalSingleton<FadeManager>
{

	[SerializeField] private Image m_blackPanel = null;
	[SerializeField] private float m_fadeSpeed = 0f;
	[SerializeField] private float m_timeFullBlackWhenOpenedScene = 0f;

	private bool m_startingTimerDone = false;
	private float m_currentStartingTimer = 0f;

	private FadeState m_currentFadeState = FadeState.None;

	private float m_newAlpha = 0f;

	[SerializeField] private float m_percentajeOfFadeForPlayerToMove = 0f;
	private bool m_playerCanMove = false;

	public bool PlayerCanMove { get => m_playerCanMove; set => m_playerCanMove = value; }

	// Start is called before the first frame update
	void Start()
    {
		m_currentFadeState = FadeState.FadingOut;
		m_playerCanMove = false;
	}

    // Update is called once per frame
    void Update()
    {
		switch (m_currentFadeState)
		{
			case FadeState.FadingIn:
				{
					m_newAlpha = m_blackPanel.color.a + m_fadeSpeed * Time.deltaTime;
					if (m_newAlpha >= 1)
					{
						m_newAlpha = 1;
						m_currentFadeState = FadeState.None;
						m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
						LevelManager.Instance.RestartLevel();
					}
					m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
				}
				break;
			case FadeState.FadingOut:
				{
					if (m_startingTimerDone)
					{
						m_newAlpha = m_blackPanel.color.a - m_fadeSpeed * Time.deltaTime;

						if(m_newAlpha <= m_percentajeOfFadeForPlayerToMove && !m_playerCanMove)
						{
							m_playerCanMove = true;
						}

						if(m_newAlpha <= 0)
						{
							m_newAlpha = 0;
							m_currentFadeState = FadeState.None;
						}
						m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
					}
					else
					{
						m_currentStartingTimer = m_currentStartingTimer - Time.deltaTime;
						if (m_currentStartingTimer <= 0)
						{
							m_startingTimerDone = true;
						}
					}
				}
				break;
			case FadeState.None:
				break;
			default:
				break;
		}
	}

	public void StartFadingIn()
	{
		m_currentFadeState = FadeState.FadingIn;
		m_playerCanMove = false;
	}

	public void StartFadingOut()
	{
		m_currentFadeState = FadeState.FadingOut;
		m_currentStartingTimer = m_timeFullBlackWhenOpenedScene;
		m_startingTimerDone = false;
	}



}