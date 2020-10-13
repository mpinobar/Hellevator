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

    List<string> adjacentScenes;
    LevelLoadManager centralScene;



    public CheckPoint LastCheckPoint { get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }

	public bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
	public List<Vector3> CheckPoints { get => m_checkPoints; set => m_checkPoints = value; }
    public LevelLoadManager CentralScene { get => centralScene; set => centralScene = value; }


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
            SceneManager.LoadSceneAsync(centralScene.AdjacentScenes[i], LoadSceneMode.Additive);            
        }
        if (adjacentScenes == null)
        {
            adjacentScenes = new List<string>();
        }
        for (int i = 0; i < newCentralScene.AdjacentScenes.Count; i++)
        {
            adjacentScenes.Add(newCentralScene.AdjacentScenes[i]);
        }
    }

    public void ChangeCentralScene(LevelLoadManager newCentralScene)
    {
        if(centralScene != newCentralScene)
        {
            if (adjacentScenes == null)
            {
                adjacentScenes = new List<string>();
            }

            adjacentScenes.Add(centralScene.ThisSceneName);
            centralScene = newCentralScene;

            //la que ahora es central ya no es adyacente
            adjacentScenes.Remove(centralScene.ThisSceneName);
            //for (int i = 0; i < adjacentScenes.Count; i++)
            //{
            //    if (adjacentScenes[i] == newCentralScene.ThisSceneName)
            //    {
            //        //la que antes era la central ahora es una adyacente
            //        adjacentScenes.Add(centralScene.ThisSceneName);
            //        centralScene = newCentralScene;

            //        //la que ahora es central ya no es adyacente
            //        adjacentScenes.Remove(centralScene.ThisSceneName);
            //        break;
            //    }
            //}

            Debug.LogError("New central scene is " + newCentralScene.ThisSceneName);
            for (int i = 0; i < adjacentScenes.Count; i++)
            {
                if (!centralScene.AdjacentScenes.Contains(adjacentScenes[i]))
                {
                    Debug.LogError("Removing from loaded scenes: " + adjacentScenes[i]);
                    SceneManager.UnloadSceneAsync(adjacentScenes[i]);
                    adjacentScenes.Remove(adjacentScenes[i]);
                    i--;
                }
                else
                {
                    Debug.LogError("Keeping scene: " + adjacentScenes[i]);
                }
            }

            for (int i = 0; i < centralScene.AdjacentScenes.Count; i++)
            {
                bool isNewAdjacentSceneToCentralLoaded = false;
                for (int j = 0; j < adjacentScenes.Count; j++)
                {
                    if (adjacentScenes[j] == centralScene.AdjacentScenes[i])
                    {
                        isNewAdjacentSceneToCentralLoaded = true;
                    }
                }
                if (!isNewAdjacentSceneToCentralLoaded)
                {
                    SceneManager.LoadSceneAsync(centralScene.AdjacentScenes[i], LoadSceneMode.Additive);
                }
            }
        }        

    }


}
