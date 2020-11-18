using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : PersistentSingleton<LevelManager>
{
    
    [SerializeField] Puzzle[] m_scenePuzzles;
    private List<Vector3> m_checkPoints;

    private CheckPoint m_lastCheckPoint;
    private string m_checkPointSceneToLoad;
    private bool m_isRestarting;

    AsyncOperation m_loadingScene;

    //[SerializeField] private GameObject m_fade = null;

    List<string> m_adjacentScenes;
    LevelLoadManager m_centralScene;

    [SerializeField] GameObject cameraPrefab;
    
    bool m_isSwitchingToNewScene;
    bool m_canLoad;
    string m_newSceneName;
    string m_previousScene;

    bool m_hasKitchenKey;
    public CheckPoint LastCheckPoint
    {
        get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }

    public bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
    public List<Vector3> CheckPoints { get => m_checkPoints; set => m_checkPoints = value; }
    public LevelLoadManager CentralSceneLoadManager { get => m_centralScene; set => m_centralScene = value; }
    public bool HasKitchenKey { get => m_hasKitchenKey; set => m_hasKitchenKey = value; }
    public bool CanLoad { get => m_canLoad; set => m_canLoad = value; }
    public string PreviousScene { get => m_previousScene; set => m_previousScene = value; }

    private void Start()
    {
        PlayerPrefs.DeleteAll();
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
        //bool foundInside = false;
        //for (int i = 0; i < m_checkPoints.Count; i++)
        //{
        //    if (m_checkPoints[i] == value.transform.position)
        //    {
        //        foundInside = true;
        //    }
        //}
        //if (!foundInside)
        //{
        //    m_checkPoints.Add(value.transform.position);
        //    m_lastCheckPoint = value;
        //}
        m_checkPoints.Add(value.transform.position);
        m_lastCheckPoint = value;
        m_checkPointSceneToLoad = m_lastCheckPoint.SceneToLoad;
    }
        

    private void Update()
    {
  
        if (m_isRestarting)
        {
            //if (m_loadingScene.isDone)
            //{
            //UpdateLastCheckPointReference();


            //CameraManager.Instance.CurrentCamera.enabled = false;
            //CameraManager.Instance.CurrentCamera.transform.SetPositionAndRotation(new Vector3(m_lastCheckPoint.transform.position.x, m_lastCheckPoint.transform.position.y, CameraManager.Instance.CurrentCamera.transform.position.z), CameraManager.Instance.CurrentCamera.transform.rotation);
            //CameraManager.Instance.CurrentCamera.enabled = true;
            //ParalaxManager.Instance.SetUpSceneParalax();

            //if (PossessionManager.Instance.ControlledDemon != null)
            //{
            //    PossessionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
            //}
            //m_lastCheckPoint.SpawnPlayer();

            m_isRestarting = false;
            Time.timeScale = 1;
            //}
        }

        if (m_isSwitchingToNewScene)
        {
            
            if(m_loadingScene.progress >= 0.9f && m_canLoad)
            {
                //Debug.LogError("Done loading scene " + m_newSceneName);
                //Debug.LogError(PossessionManager.Instance.ControlledDemon.gameObject.scene.name);
                m_isSwitchingToNewScene = false;
				AsyncOperation op = SceneManager.UnloadSceneAsync(PossessionManager.Instance.ControlledDemon.gameObject.scene.name);
				PossessionManager.Instance.MoveMainCharacterToScene(SceneManager.GetSceneByName("PersistentGameObjects"));
                m_loadingScene.allowSceneActivation = true;
                CameraManager.Instance.FadeOut();
                
                //Debug.LogError(PossessionManager.Instance.ControlledDemon.gameObject.scene.name);
                //op.completed += UnloadCompleted;
            }
        }
    }
    
    private void UnloadCompleted(AsyncOperation obj)
    {
        //PossessionManager.Instance.MoveMainCharacterToScene(SceneManager.GetSceneByName(m_newSceneName));
        //Debug.LogError("Unloaded successfully");
        //Debug.LogError(PossessionManager.Instance.ControlledDemon.gameObject.scene.name);

    }

    /// <summary>
    /// Picks up the reference again of the last entered checkpoint among all checkpoints
    /// </summary>
    private void UpdateLastCheckPointReference()
    {
        CheckPoint[] cps = FindObjectsOfType<CheckPoint>();

        for (int i = 0; i < cps.Length; i++)
        {
            if (m_checkPoints[m_checkPoints.Count - 1] == cps[i].transform.position)
            {
                m_lastCheckPoint = cps[i];

                return;
            }
        }

    }

    /// <summary>
    /// Resets the level and spawns the player at the checkpoints
    /// </summary>
    public void StartRestartingLevel()
    {
        CameraManager.Instance.FadeIn();
        FadeManager.IsRestarting = true;
    }
    public void RestartLevel()
    {
        string nameToLoad = "R.1";

        if (m_lastCheckPoint)
        {
            //en este caso tengo que reiniciar la misma sala en la que he muerto
            nameToLoad = m_lastCheckPoint.SceneToLoad;
        }
        else if(m_checkPointSceneToLoad != null && m_checkPointSceneToLoad.Length > 0)
        {
            //he muerto en una sala distinta al ultimo checkpoint en el que he muerto
            nameToLoad = m_checkPointSceneToLoad;
        }
        AsyncOperation op = SceneManager.LoadSceneAsync(nameToLoad);
        if(m_adjacentScenes != null)
         m_adjacentScenes.Clear();
        CentralSceneLoadManager = null;
        op.completed += LoadCompletedRestart;

        if (m_checkPoints != null && m_checkPoints.Count > 0)
        {
            m_isRestarting = true;
        }
    }

    private void LoadCompletedRestart(AsyncOperation obj)
    {
        UpdateLastCheckPointReference();


        CameraManager.Instance.CurrentCamera.enabled = false;
        if(m_lastCheckPoint)
        CameraManager.Instance.CurrentCamera.transform.SetPositionAndRotation(new Vector3(m_lastCheckPoint.transform.position.x, m_lastCheckPoint.transform.position.y, CameraManager.Instance.CurrentCamera.transform.position.z), CameraManager.Instance.CurrentCamera.transform.rotation);
        CameraManager.Instance.CurrentCamera.enabled = true;
        CameraManager.Instance.SetupParallax();
        
        if (PossessionManager.Instance.ControlledDemon != null)
        {
            PossessionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
        }
        if (!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        {
            SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
        }
        if (m_lastCheckPoint)
            m_lastCheckPoint.SpawnPlayer();
        CameraManager.Instance.FadeOut();
        MusicManager.Instance.StartGameplayMusic();
    }


    public void LoadCentralSceneFirstTime(LevelLoadManager newCentralScene)
    {
        CentralSceneLoadManager = newCentralScene;

        if (!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        {
            SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
        }

        //for (int i = 0; i < newCentralScene.AdjacentScenes.Count; i++)
        //{
        //    AsyncOperation op = SceneManager.LoadSceneAsync(m_centralScene.AdjacentScenes[i]);
        //    op.allowSceneActivation = false;
        //    m_adjScenes.Add(CentralScene.AdjacentScenes[i],op);
        //    //m_adjScenes[CentralScene.AdjacentScenes[i]].allowSceneActivation = false;
        //}
        //if (m_adjacentScenes == null)
        //{
        //    m_adjacentScenes = new List<string>();
        //}
        //for (int i = 0; i < newCentralScene.AdjacentScenes.Count; i++)
        //{
        //    m_adjacentScenes.Add(newCentralScene.AdjacentScenes[i]);

        //}


        //SceneManager.MoveGameObjectToScene(CameraManager.Instance.gameObject, SceneManager.GetSceneByName(m_centralScene.ThisSceneName));
    }

    public void ChangeCentralScene(LevelLoadManager newCentralScene)
    {
        if (m_centralScene != newCentralScene)
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
            //SceneManager.MoveGameObjectToScene(CameraManager.Instance.gameObject, SceneManager.GetSceneByName(m_centralScene.ThisSceneName));

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

    
    public void SwitchToAdjacentScene(string newSceneName)
    {
        m_previousScene = PossessionManager.Instance.ControlledDemon.gameObject.scene.name;
        CameraManager.Instance.FadeIn();
        m_newSceneName = newSceneName;
        m_loadingScene = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);
        m_loadingScene.allowSceneActivation = false;
        m_loadingScene.completed += LoadSwitchSceneCompleted;
        m_isSwitchingToNewScene = true;
        FadeManager.IsRestarting = false;
        CanLoad = false;
        
        
    }

    private void LoadSwitchSceneCompleted(AsyncOperation obj)
    {
        PossessionManager.Instance.MoveMainCharacterToScene(SceneManager.GetSceneByName(m_newSceneName));
        CameraManager.Instance.ChangeFocusOfMainCameraTo(PossessionManager.Instance.ControlledDemon.transform);
    }
}
