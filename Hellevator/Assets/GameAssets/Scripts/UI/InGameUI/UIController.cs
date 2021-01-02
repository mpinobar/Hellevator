using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : PersistentSingleton<UIController>
{
    [SerializeField] Selectable m_selected;

    [Header("Parent references")]
    [SerializeField] Canvas m_canvas;
    [SerializeField] GameObject m_pausePanel;
    [SerializeField] GameObject m_inventoryPanel;
    [SerializeField] GameObject m_mapPanel;
    [SerializeField] GameObject m_bestiaryPanel;
    [SerializeField] GameObject m_collectiblesPanel;
    [SerializeField] GameObject m_settingsPanel;

    [SerializeField] GameplayUIController m_gameplayPanel;

    [Header("Pause buttons references")]
    [SerializeField] Button m_resumeButton;
    [SerializeField] Button m_exitButton;
    [SerializeField] Button m_mapButton;
    [SerializeField] Button m_inventoryButton;
    [SerializeField] Button m_bestiaryButton;
    [SerializeField] Button m_collectiblesButton;
    [SerializeField] Button m_settingsButton;


    GameObject m_activePanel;
    bool m_hasMovedVerticallyOnMenu;
    bool m_hasMovedHorizontallyOnMenu;

    public Selectable Selected
    {
        get => m_selected;
        set
        {
            if (m_selected != null && value != m_selected)
            {
                m_selected.OnDeselected();
            }
            m_selected = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //if (!m_canvas)
        //    m_canvas = transform.GetChild(0).gameObject.GetComponent<Canvas>();
        //m_canvas.gameObject.SetActive(false);
        m_resumeButton.onClick.AddListener(Resume);
        m_inventoryButton.onClick.AddListener(ShowInventory);
        m_mapButton.onClick.AddListener(ShowMap);
        m_collectiblesButton.onClick.AddListener(ShowCollectibles);
        m_settingsButton.onClick.AddListener(ShowSettings);
        m_bestiaryButton.onClick.AddListener(ShowBestiary);
        m_exitButton.onClick.AddListener(Exit);
        ShowGameplayUI();
    }

    public void Resume()
    {
        if (m_activePanel)
        {
            m_activePanel.SetActive(false);
        }
        m_canvas.gameObject.SetActive(false);

        Time.timeScale = 1f;
        CameraManager.Instance.HideUIEffects();
        InputManager.Instance.IsInMenu = false;
        //ShowGameplayUI();
        m_gameplayPanel.gameObject.SetActive(true);
        m_activePanel = m_gameplayPanel.gameObject;
    }

    public void Exit()
    {
        m_canvas.gameObject.SetActive(false);
        Time.timeScale = 1f;
        m_activePanel = null;
        LevelManager.Instance.LoadMainMenu();
        InputManager.Instance.IsInMenu = false;
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

    public void ShowGameplayUI()
    {
        for (int i = 1; i < m_canvas.transform.childCount - 2; i++)
        {
            m_canvas.transform.GetChild(i).gameObject.SetActive(false);
        }
        m_canvas.gameObject.SetActive(false);
        ShowPanel(m_gameplayPanel.gameObject);
    }

    public void ShowPauseMenu()
    {
        if (m_canvas.worldCamera == null)
        {
            m_canvas.worldCamera = Camera.main;
            m_canvas.sortingLayerName = "UI";
            m_canvas.sortingOrder = 1000;
        }
        InputManager.Instance.IsInMenu = true;
        ShowPanel(m_pausePanel);
        CameraManager.Instance.ShowUIEffects();
        Time.timeScale = 0f;

    }

    private void ShowPanel(GameObject panelToShow)
    {
        if (panelToShow != m_gameplayPanel.gameObject)
            m_canvas.gameObject.SetActive(true);
        if (panelToShow != m_activePanel)
        {
            if (m_activePanel != null && m_activePanel != m_canvas)
            {
                m_activePanel.SetActive(false);
            }
            panelToShow.SetActive(true);
            m_activePanel = panelToShow;
        }
    }


    public void NavigateMenu(float xInput, float yInput)
    {

        if (!m_hasMovedHorizontallyOnMenu)
        {
            if (xInput > 0)
            {
                Selected.NavigateRight();
                m_hasMovedHorizontallyOnMenu = true;
            }
            if (xInput < 0)
            {
                Selected.NavigateLeft();
                m_hasMovedHorizontallyOnMenu = true;
            }
        }
        if (!m_hasMovedVerticallyOnMenu)
        {
            if (yInput < 0)
            {
                Selected.NavigateDown();
                m_hasMovedVerticallyOnMenu = true;
            }
            if (yInput > 0)
            {
                Selected.NavigateUp();
                m_hasMovedVerticallyOnMenu = true;
            }
        }

        if (xInput == 0)
        {
            m_hasMovedHorizontallyOnMenu = false;
        }
        if (yInput == 0)
        {
            m_hasMovedVerticallyOnMenu = false;
        }
    }

    public void TryDiscoverNewZone(string zone)
    {
        if (!PlayerPrefs.HasKey(zone) || PlayerPrefs.GetInt(zone) != 2)
        {
            Debug.LogError("Discovering zone " + zone);
            PlayerPrefs.SetInt(zone, 1);
            //Debug.LogError(PlayerPrefs.GetInt(zone));
        }
    }

    public void PossessionDecisionTime(float time)
    {
        m_gameplayPanel.PossessionDecisionTime(time);
    }

    public void EndDecisionTime()
    {
        m_gameplayPanel.HideDecisionTimeBar();
    }
    public void UnlockMap()
    {
        string letter = "K";
        for (int i = 1; i <= 9; i++)
        {
            string number = ""+i;
            string unlock = letter + "."+number;
            TryDiscoverNewZone(unlock);
        }
        letter = "R";
        for (int i = 1; i <= 15; i++)
        {
            string number = ""+i;
            string unlock = letter + "."+number;
            TryDiscoverNewZone(unlock);
        }

    }
}
