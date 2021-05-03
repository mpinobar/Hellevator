using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    [SerializeField] bool m_deletesPlayerPrefs;
    MainMenuCanvasController m_canvasController;
    [SerializeField] AudioClip m_buttonSoundClip;
    [SerializeField] bool m_goesToK1;
    [SerializeField] bool m_goesToBoss;
    bool loaded;
    private void Start()
    {
        m_canvasController = FindObjectOfType<MainMenuCanvasController>();
    }

    private void Update()
    {
        if (m_canvasController.HasFadedWhenLoading)
        {
            if (!loaded)
            {
                //SceneManager.LoadSceneAsync("PersistentGameObjects");
                loaded = true;                               
            }
        }
    }

    public void OnPlayPressed()
    {
        if(m_buttonSoundClip)
        AudioManager.Instance.PlayAudioSFX(m_buttonSoundClip, false);
        m_canvasController.ChangeState(MenuCameraState.Loading);
        if (m_goesToBoss)
        {
            LevelManager.Instance.CheckPointSceneToLoad = "BossRoom";
        }
        else if (m_goesToK1)
        {
            LevelManager.Instance.CheckPointSceneToLoad = "K.1";
        }
        else
        {
            if (m_deletesPlayerPrefs)
            {
                PossessionManager.Instance.MultiplePossessionIsUnlocked = false;
                LevelManager.Instance.LastCheckPoint = null;
                LevelManager.Instance.CheckPointSceneToLoad = null;
                PlayerPrefs.DeleteAll();
            }

            if (!PlayerPrefs.HasKey("CPScene") || PlayerPrefs.GetString("CPScene") == null || PlayerPrefs.GetString("CPScene") == "")
                LevelManager.Instance.CheckPointSceneToLoad = AppScenes.INITIAL_SCENE;
        }
        
    }
}
