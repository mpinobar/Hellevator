using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxManager : TemporalSingleton<ParalaxManager>
{
	[SerializeField] private Paralax[] m_paralaxes = new Paralax[3];



	public void SetUpSceneParalax()
	{
		for (int i = 0; i < m_paralaxes.Length; i++)
		{
			m_paralaxes[i].SetUpParalax();
		}
	}

}
