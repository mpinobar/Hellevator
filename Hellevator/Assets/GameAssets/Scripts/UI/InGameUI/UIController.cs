using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : PersistentSingleton<UIController>
{
    [Header("Parent references")]
    [SerializeField] GameObject m_canvas;
    [SerializeField] GameObject m_pausePanel;
    [SerializeField] GameObject m_inventoryPanel;
    [SerializeField] GameObject m_mapPanel;
    [SerializeField] GameObject m_bestiaryPanel;
    [SerializeField] GameObject m_collectiblesPanel;
    [SerializeField] GameObject m_settingsPanel;

    [Header("Pause buttons references")]
    [SerializeField] Button m_resumeButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Button m_mapButton;
    [SerializeField] Button m_inventoryButton;
    [SerializeField] Button m_bestiaryButton;
    [SerializeField] Button m_collectiblesButton;
    [SerializeField] Button m_settingsButton;


    GameObject m_activePanel;

    // Start is called before the first frame update
    void Start()
    {
        if (!m_canvas)
            m_canvas = transform.GetChild(0).gameObject;
        m_canvas.SetActive(false);
        m_resumeButton.onClick.AddListener(Resume);
        m_inventoryButton.onClick.AddListener(ShowInventory);
        m_mapButton.onClick.AddListener(ShowMap);
        m_collectiblesButton.onClick.AddListener(ShowCollectibles);
        m_settingsButton.onClick.AddListener(ShowSettings);
        m_bestiaryButton.onClick.AddListener(ShowBestiary);
        m_exitButton.onClick.AddListener(Exit);

    }

    public void Resume()
    {
        m_canvas.SetActive(false);
        m_activePanel = null;
        Time.timeScale = 1f;
    }

    public void Exit()
    {
        m_canvas.SetActive(false);
        Time.timeScale = 1f;
        m_activePanel = null;
        LevelManager.Instance.LoadMainMenu();
    }

    public void ShowInventory()
    {
        ShowPanel(m_inventoryPanel);
    }

    public void ShowMap()
    {
        ShowPanel(m_mapPanel);
    }

    public void ShowCollectibles()
    {
        ShowPanel(m_collectiblesPanel);
    }

    public void ShowSettings()
    {
        ShowPanel(m_settingsPanel);
    }

    public void ShowBestiary()
    {
        ShowPanel(m_bestiaryPanel);
    }

    public void ShowPauseMenu()
    {
        ShowPanel(m_pausePanel);
        Time.timeScale = 0f;
    }

    private void ShowPanel(GameObject panelToShow)
    {
        m_canvas.SetActive(true);
        if(panelToShow != m_activePanel)
        {
            if(m_activePanel != null && m_activePanel != m_canvas)
            {
                m_activePanel.SetActive(false);
            }
            panelToShow.SetActive(true);
            m_activePanel = panelToShow;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ShowPauseMenu();
        }
    }
}
