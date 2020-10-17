﻿using System;
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

    List<string> m_adjacentScenes;
    LevelLoadManager m_centralScene;



    public CheckPoint LastCheckPoint { get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }

	public bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
	public List<Vector3> CheckPoints { get => m_checkPoints; set => m_checkPoints = value; }
    public LevelLoadManager CentralScene { get => m_centralScene; set => m_centralScene = value; }


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
				ParalaxManager.Instance.SetUpSceneParalax();

				if (PossessionManager.Instance.ControlledDemon != null)
                {
                    PossessionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
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
        Debug.LogError("Restarting");
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

    public void LoadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void LoadLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void LoadCentralSceneFirstTime(LevelLoadManager newCentralScene)
    {
        CentralScene = newCentralScene;
        for (int i = 0; i < newCentralScene.AdjacentScenes.Count; i++)
        {
            SceneManager.LoadSceneAsync(m_centralScene.AdjacentScenes[i], LoadSceneMode.Additive);            
        }
        if (m_adjacentScenes == null)
        {
            m_adjacentScenes = new List<string>();
        }
        for (int i = 0; i < newCentralScene.AdjacentScenes.Count; i++)
        {
            m_adjacentScenes.Add(newCentralScene.AdjacentScenes[i]);
        }

        if(!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
    }

    public void ChangeCentralScene(LevelLoadManager newCentralScene)
    {
        if(m_centralScene != newCentralScene)
        {
            if (m_adjacentScenes == null)
            {
                m_adjacentScenes = new List<string>();
            }

            m_adjacentScenes.Add(m_centralScene.ThisSceneName);
            m_centralScene = newCentralScene;

            //la que ahora es central ya no es adyacente
            m_adjacentScenes.Remove(m_centralScene.ThisSceneName);

            PossessionManager.Instance.MoveDemonsToCentralScene(SceneManager.GetSceneByName(m_centralScene.ThisSceneName));
            SceneManager.MoveGameObjectToScene(CameraManager.Instance.gameObject, SceneManager.GetSceneByName(m_centralScene.ThisSceneName));
            
            //Debug.LogError("New central scene is " + newCentralScene.ThisSceneName);

            //Descargar las escenas que ya no se necesitan porque no se encuentran entre las adyacentes de la nueva central
            for (int i = 0; i < m_adjacentScenes.Count; i++)
            {
                if (!m_centralScene.AdjacentScenes.Contains(m_adjacentScenes[i]))
                {
                    //Debug.LogError("Removing from loaded scenes: " + adjacentScenes[i]);
                    SceneManager.UnloadSceneAsync(m_adjacentScenes[i]);
                    m_adjacentScenes.Remove(m_adjacentScenes[i]);
                    i--;
                }
                else
                {
                    //Debug.LogError("Keeping scene: " + adjacentScenes[i]);
                }
            }

            //Cargar las escenas que se necesitan por ser adyacentes a la nueva central y que no están cargadas
            for (int i = 0; i < m_centralScene.AdjacentScenes.Count; i++)
            {
                if (!m_adjacentScenes.Contains(m_centralScene.AdjacentScenes[i]))
                {
                    if (!SceneManager.GetSceneByName(m_centralScene.AdjacentScenes[i]).isLoaded)
                    {
                        SceneManager.LoadSceneAsync(m_centralScene.AdjacentScenes[i], LoadSceneMode.Additive);
                        m_adjacentScenes.Add(m_centralScene.AdjacentScenes[i]);
                    }
                    else
                    {
                        //Debug.LogError("Scene already loaded: " + centralScene.AdjacentScenes[i]);
                    }                    
                }
            }
        }       
    }


}
