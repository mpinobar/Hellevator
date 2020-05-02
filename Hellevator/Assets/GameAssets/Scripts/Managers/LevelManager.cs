using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : PersistentSingleton<LevelManager>
{
    public List<Vector3> m_checkPoints;

    public CheckPoint m_lastCheckPoint;

    private bool m_isRestarting;

    AsyncOperation m_loadingScene;

    public CheckPoint LastCheckPoint { get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }


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
                if(PosesionManager.Instance.ControlledDemon != null)
                {
                    PosesionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
                }
                UpdateLastCheckPointReference();
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
                return;
            }
        }
        
    }

    /// <summary>
    /// Resets the level and spawns the player at the checkpoints
    /// </summary>
    public void RestartLevel()
    {
        m_loadingScene = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        if( m_checkPoints != null && m_checkPoints.Count > 0)
        {
            m_isRestarting = true;
        }
        
    }
}
