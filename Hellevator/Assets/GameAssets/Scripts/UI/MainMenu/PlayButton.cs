using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{
    MainMenuCanvasController m_canvasController;
    [SerializeField] AudioClip m_buttonSoundClip;

    private void Start()
    {
        m_canvasController = FindObjectOfType<MainMenuCanvasController>();
    }

    private void Update()
    {
        if (m_canvasController.HasFadedWhenLoading)
        {
            SceneManager.LoadScene(AppScenes.LEVEL_01);
        }
    }

    public void OnPlayPressed()
    {
        MusicManager.Instance.PlayAudioSFX(m_buttonSoundClip, false);
        m_canvasController.ChangeState(MenuCameraState.Loading);        
    }
}
