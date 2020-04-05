﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balanza : MonoBehaviour
{
	[SerializeField] private Transform m_minHeight = null;
	[SerializeField] private Transform m_maxHeight = null;
	private float m_maxY = 0;
	private float m_minY = 0;
	private float m_midPointY = 0;

	[SerializeField] private GameObject m_leftScale = null;
	[SerializeField] private GameObject m_rightScale = null;
	private PesoBalanza m_weightLeft = null;
	private PesoBalanza m_weightRight = null;

	[SerializeField] private float m_minWeightDifferenceForMaxHeight = 0;
	[SerializeField] private float m_speed = 0;

	private BalanzaState m_currentState = BalanzaState.Equal;
	

	private void Awake()
	{
		m_weightLeft = m_leftScale.GetComponent<PesoBalanza>();
		m_weightRight = m_rightScale.GetComponent<PesoBalanza>();

		m_maxY = m_maxHeight.position.y;
		m_minY = m_minHeight.position.y;
		m_midPointY = (m_maxY + m_minY) / 2f;
	}

    // Update is called once per frame
    void Update()
    {
		//Calcular Diferencia de Peso

		float weightDiference = m_weightLeft.CurrentWeight - m_weightRight.CurrentWeight; //Si es negativo la izquierda pesa más. 

		if(weightDiference == 0)
		{
			m_currentState = BalanzaState.Equal;
		}
		else if(weightDiference > 0)
		{
			m_currentState = BalanzaState.RightIsHeavier;
		}
		else
		{
			m_currentState = BalanzaState.LeftIsHeavier;
		}

		weightDiference = Mathf.Abs(weightDiference);
		
		//Mover segun diferencia de peso

		float percentage = weightDiference / m_minWeightDifferenceForMaxHeight;

		if(weightDiference > 1)
		{
			weightDiference = 1;
		}

		float yToMove = (m_maxY - m_midPointY) * percentage;

		switch (m_currentState)
		{
			case BalanzaState.Equal:
				{
					m_leftScale.transform.position = Vector2.MoveTowards(m_leftScale.transform.position, new Vector2(m_leftScale.transform.position.x, m_midPointY), m_speed * Time.deltaTime);
					m_rightScale.transform.position = Vector2.MoveTowards(m_rightScale.transform.position, new Vector2(m_rightScale.transform.position.x, m_midPointY), m_speed * Time.deltaTime);
				}
				break;
			case BalanzaState.RightIsHeavier:
				{
					m_leftScale.transform.position = Vector2.MoveTowards(m_leftScale.transform.position, new Vector2(m_leftScale.transform.position.x, m_midPointY + yToMove), m_speed * 2* Time.deltaTime);
					m_rightScale.transform.position = Vector2.MoveTowards(m_rightScale.transform.position, new Vector2(m_rightScale.transform.position.x, m_midPointY - yToMove), m_speed * 2 * Time.deltaTime);
				}
				break;
			case BalanzaState.LeftIsHeavier:
				{
					m_leftScale.transform.position = Vector2.MoveTowards(m_leftScale.transform.position, new Vector2(m_leftScale.transform.position.x, m_midPointY - yToMove), m_speed * 2 * Time.deltaTime);
					m_rightScale.transform.position = Vector2.MoveTowards(m_rightScale.transform.position, new Vector2(m_rightScale.transform.position.x, m_midPointY + yToMove), m_speed * 2 * Time.deltaTime);
				}
				break;
			default:
				break;
		}
	}
}