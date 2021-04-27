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

    /// <summary>
    /// Indice de la mascara del penitente que ha muerto.
    /// </summary>
    private int m_maskIndex = -1;

    AsyncOperation m_loadingScene;

    //[SerializeField] private GameObject m_fade = null;
    public static Action LevelLoaded;

    List<string> m_adjacentScenes;
    LevelLoadManager m_centralScene;

    [SerializeField] GameObject cameraPrefab;

    bool m_isSwitchingToNewScene;
    bool m_canLoad;
    string m_newSceneName;
    string m_previousScene;

    string m_checkpointPlayerPrefsID = "CPScene";

	bool m_hasKitchenKey;
    public CheckPoint LastCheckPoint
    {
        get => m_lastCheckPoint;

        set
        {
            m_lastCheckPoint = value;
        }
    }

    public string PlayerSceneName { get => m_newSceneName; }
    public bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
    public List<Vector3> CheckPoints { get => m_checkPoints; set => m_checkPoints = value; }
    public LevelLoadManager CentralSceneLoadManager { get => m_centralScene; set => m_centralScene = value; }
    public bool HasKitchenKey { get => m_hasKitchenKey; set => m_hasKitchenKey = value; }
    public bool CanLoad { get => m_canLoad; set => m_canLoad = value; }
    public string PreviousScene { get => m_previousScene; set => m_previousScene = value; }
    public string CheckPointSceneToLoad { get => m_checkPointSceneToLoad; set => m_checkPointSceneToLoad = value; }
    public string NewSceneName { get => m_newSceneName; }
    public int MaskIndex { get => m_maskIndex; set { /*Debug.LogError(value);*/ m_maskIndex = value; } }

    public override void Awake()
    {
        base.Awake();
        if (PlayerPrefs.HasKey(m_checkpointPlayerPrefsID) && PlayerPrefs.GetString(m_checkpointPlayerPrefsID) != null && PlayerPrefs.GetString(m_checkpointPlayerPrefsID) != "")
            m_checkPointSceneToLoad = PlayerPrefs.GetString(m_checkpointPlayerPrefsID);
    }

    private void Start()
    {
		//PlayerPrefs.DeleteAll
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

        if (!m_checkPoints.Contains(value.transform.position))
            m_checkPoints.Add(value.transform.position);

        m_lastCheckPoint = value;
        m_checkPointSceneToLoad = m_lastCheckPoint.SceneToLoad;
    }

    public void SaveSceneToLoad(string scene)
    {
        PlayerPrefs.SetString(m_checkpointPlayerPrefsID, scene);
    }

    private void Update()
    {

        //if (m_isRestarting)
        //{

        //    m_isRestarting = false;
        //    Time.timeScale = 1;

        //}

        if (m_isSwitchingToNewScene && m_loadingScene.progress >= 0.9f && !FadeManager.IsInTransition)
        {
            m_loadingScene.allowSceneActivation = true;

            m_isSwitchingToNewScene = false;

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
    public void StartRestartingLevelWithDelay()
    {
        if (m_isRestarting)
            return;
        StopAllCoroutines();
        StartCoroutine(DelayAndRestart(2.5f));
    }

    public void StartRestartingLevelNoDelay()
    {
        if (m_isRestarting)
            return;
        StopAllCoroutines();
        StartCoroutine(DelayAndRestart(0));
    }
    IEnumerator DelayAndRestart(float time)
    {
        m_isRestarting = true;
        yield return new WaitForSeconds(time);
        CameraManager.Instance.FadeIn();
        FadeManager.IsRestarting = true;
    }

    public void RestartLevel()
    {
        //Debug.LogError("Restart");
        Cursor.visible = false;
        string nameToLoad = AppScenes.INITIAL_SCENE;

        if (m_lastCheckPoint)
        {
            //en este caso tengo que reiniciar la misma sala en la que he muerto
            nameToLoad = m_lastCheckPoint.SceneToLoad;
        }
        else if (m_checkPointSceneToLoad != null && m_checkPointSceneToLoad.Length > 0)
        {
            //he muerto en una sala distinta al ultimo checkpoint en el que he muerto
            nameToLoad = m_checkPointSceneToLoad;
        }
        AsyncOperation op = SceneManager.LoadSceneAsync(nameToLoad);
		if (m_adjacentScenes != null)
            m_adjacentScenes.Clear();
        CentralSceneLoadManager = null;
        op.completed += LoadCompletedRestart;

        if (m_checkPoints != null && m_checkPoints.Count > 0)
        {
            m_isRestarting = true;
        }
        PossessionManager.Instance.ClearMultiplePossession();
        InputManager.Instance.ThrowingHead = false;
    }

    private void LoadCompletedRestart(AsyncOperation obj)
    {
        //Debug.LogError("Has Checkpoint?" + (m_lastCheckPoint != null));
        if (m_checkPoints != null && m_checkPoints.Count > 0)
            UpdateLastCheckPointReference();
        //Debug.LogError("Has Checkpoint?" + (m_lastCheckPoint != null));


        CameraManager.Instance.CurrentCamera.enabled = false;
        if (m_lastCheckPoint)
            CameraManager.Instance.CurrentCamera.transform.SetPositionAndRotation(new Vector3(m_lastCheckPoint.transform.position.x, m_lastCheckPoint.transform.position.y, CameraManager.Instance.CurrentCamera.transform.position.z), CameraManager.Instance.CurrentCamera.transform.rotation);
        CameraManager.Instance.CurrentCamera.enabled = true;
        CameraManager.Instance.SetupParallax();

        if (PossessionManager.Instance.ControlledDemon != null)
        {
            PossessionManager.Instance.ControlledDemon.SetNotControlledByPlayer();
        }
        //if (!SceneManager.GetSceneByName("PersistentGameObjects").IsValid())
        //{
        //    SceneManager.LoadSceneAsync("PersistentGameObjects", LoadSceneMode.Additive);
        //}
        if (m_lastCheckPoint)
        {
            //Debug.LogError("Trying to spawn player");
            m_lastCheckPoint.SpawnPlayer();
        }
        else
        {
            //Debug.LogError("Not found checkpoint to spawn player from");
            CheckPoint cp = FindObjectOfType<CheckPoint>();
            if (cp)
                cp.SpawnPlayer();
        }
        LevelLoaded?.Invoke();
        CameraManager.Instance.FadeOut();
        AudioManager.Instance.StartGameplayMusic();
        m_isRestarting = false;
        
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
        m_loadingScene = SceneManager.LoadSceneAsync(m_newSceneName.Split('_')[0], LoadSceneMode.Additive);
        m_loadingScene.allowSceneActivation = false;
        m_loadingScene.completed += LoadSwitchSceneCompleted;
        m_isSwitchingToNewScene = true;
        FadeManager.IsInTransition = true;
        FadeManager.IsRestarting = false;
        CanLoad = false;
        UIController.Instance.TryDiscoverNewZone(m_newSceneName.Split('_')[0]);
        //Debug.LogError("Started loading scene " + m_newSceneName);
    }

    private void LoadSwitchSceneCompleted(AsyncOperation obj)
    {
        PossessionManager.Instance.MoveMainCharacterToScene(SceneManager.GetSceneByName(m_newSceneName.Split('_')[0]));
        CameraManager.Instance.ChangeFocusOfCurrentActiveCameraTo(PossessionManager.Instance.ControlledDemon.transform);
        PossessionManager.Instance.RemovePossessionFromExtraDemons();
        SceneManager.UnloadSceneAsync(PreviousScene);
        CameraManager.Instance.FadeOut();
        //Debug.LogError("Finished loading scene" + NewSceneName);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(AppScenes.MENU_SCENE);
    }
}
