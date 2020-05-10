using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : PersistentSingleton<LevelManager>
{
	private List<Vector3> m_checkPoints;

	public CheckPoint m_lastCheckPoint;

    private bool m_isRestarting;

    AsyncOperation m_loadingScene;
	
	[SerializeField] private GameObject m_fade = null;

    public CheckPoint LastCheckPoint { get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }

	public bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
	public List<Vector3> CheckPoints { get => m_checkPoints; set => m_checkPoints = value; }


	/// <summary>
	/// Checks if the checkpoint has been already entered
	/// </summary>
	/// <param name="value">The checkpoint to check</param>
	public void SetLastCheckPoint(CheckPoint value)
    {
        if (m_checkPoints == null)
        {
            m_checkPoints = new List<Vector3>();
        }
        bool foundInside = false;
        for (int i = 0; i < m_checkPoints.Count; i++)
        {
            if(m_checkPoints[i] == value.transform.position)
            {
                foundInside = true;
            }
        }
        if (!foundInside)
        {
            m_checkPoints.Add(value.transform.position);
            m_lastCheckPoint = value;
        }
    }

    private void Update()
    {
		

        if (m_isRestarting)
        {
            if (m_loadingScene.isDone)
            {
				UpdateLastCheckPointReference();


				CameraManager.Instance.CurrentCamera.enabled = false;
				CameraManager.Instance.CurrentCamera.transform.SetPositionAndRotation(new Vector3(m_lastCheckPoint.transform.position.x, m_lastCheckPoint.transform.position.y, CameraManager.Instance.CurrentCamera.transform.position.z), CameraManager.Instance.CurrentCamera.transform.rotation);
				CameraManager.Instance.CurrentCamera.enabled = true;

				if (PosesionManager.Instance.ControlledDemon != null)
                {
                    PosesionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
                }
                m_lastCheckPoint.SpawnPlayer();

                m_isRestarting = false;
                Time.timeScale = 1;
            }
        }
    }

    /// <summary>
    /// Picks up the reference again of the last entered checkpoint among all checkpoints
    /// </summary>
    private void UpdateLastCheckPointReference()
    {
        CheckPoint[] cps = FindObjectsOfType<CheckPoint>();

        for (int i = 0; i < cps.Length; i++)
        {
            if(m_checkPoints[m_checkPoints.Count-1] == cps[i].transform.position)
            {
                m_lastCheckPoint = cps[i];
				print("CP pos = " + cps[i].transform.position);
				print("Number of CPs = " + m_checkPoints.Count);

				return;
            }
        }
        
    }

    /// <summary>
    /// Resets the level and spawns the player at the checkpoints
    /// </summary>
    public void StartRestartingLevel()
    {
		FadeManager.Instance.StartFadingIn();
    }
	public void RestartLevel()
	{
        m_loadingScene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
		
        if( m_checkPoints != null && m_checkPoints.Count > 0)
        {
            m_isRestarting = true;
        }
	}
}
