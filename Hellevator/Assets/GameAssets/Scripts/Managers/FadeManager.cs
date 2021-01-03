using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class FadeManager : MonoBehaviour
{
    [SerializeField] private Image m_blackPanel = null;
    [SerializeField] private float m_fadeSpeed = 0f;
    [SerializeField] private float m_timeFullBlackWhenOpenedScene = 0f;

    private bool m_startingTimerDone = false;
    private float m_currentStartingTimer = 0f;

    private FadeState m_currentFadeState = FadeState.None;

    private float m_newAlpha = 0f;
    private static bool m_isRestarting = false;
    [SerializeField] private float m_percentajeOfFadeForPlayerToMove = 0f;
    private bool m_playerCanMove = false;

    public bool PlayerCanMove { get => m_playerCanMove; set => m_playerCanMove = value; }
    public static bool IsRestarting { get => m_isRestarting; set => m_isRestarting = value; }
    public static bool IsInTransition { get => m_isInTransition; set => m_isInTransition = value; }

    static bool m_isInTransition = false;

    [SerializeField] SpriteRenderer[] bloodSprites;
    // Start is called before the first frame update
    void Start()
    {
        m_currentFadeState = FadeState.FadingOut;
        for (int i = 0; i < bloodSprites.Length; i++)
        {
            bloodSprites[i].gameObject.SetActive(false);
        }
        m_playerCanMove = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            StartCoroutine(SlightFade());
        }
        switch (m_currentFadeState)
        {
            case FadeState.FadingIn:
                {
                    m_newAlpha = m_blackPanel.color.a + m_fadeSpeed * Time.deltaTime;
                    if (m_newAlpha >= 1)
                    {
                        m_newAlpha = 1;
                        m_currentFadeState = FadeState.None;
                        m_isInTransition = false;
                        m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
                        if (m_isRestarting)
                        {
                            LevelManager.Instance.RestartLevel();
                            m_isRestarting = false;
                        }
                        LevelManager.Instance.CanLoad = true;
                    }
                    m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
                }
                break;
            case FadeState.FadingOut:
                {
                    if (m_startingTimerDone)
                    {
                        m_newAlpha = m_blackPanel.color.a - m_fadeSpeed * Time.deltaTime;

                        if (m_newAlpha <= m_percentajeOfFadeForPlayerToMove && !m_playerCanMove)
                        {
                            if (PossessionManager.Instance.ControlledDemon != null)
                                PossessionManager.Instance.ControlledDemon.CanMove = true;
                        }

                        if (m_newAlpha <= 0)
                        {
                            m_newAlpha = 0;
                            m_currentFadeState = FadeState.None;
                            m_isInTransition = false;
                        }
                        m_blackPanel.color = new Color(m_blackPanel.color.r, m_blackPanel.color.g, m_blackPanel.color.b, m_newAlpha);
                    }
                    else
                    {
                        m_currentStartingTimer = m_currentStartingTimer - Time.deltaTime;
                        if (m_currentStartingTimer <= 0)
                        {
                            m_startingTimerDone = true;
                        }
                    }
                }
                break;
            case FadeState.None:
                break;
            default:
                break;
        }
    }

    public void StartFadingIn()
    {
        m_isInTransition = true;
        m_blackPanel.gameObject.SetActive(true);
        m_currentFadeState = FadeState.FadingIn;
        if (PossessionManager.Instance.ControlledDemon != null)
            PossessionManager.Instance.ControlledDemon.CanMove = false;
    }

    public void StartFadingOut()
    {
        m_isInTransition = true;
        m_blackPanel.gameObject.SetActive(true);
        m_currentFadeState = FadeState.FadingOut;
        m_currentStartingTimer = m_timeFullBlackWhenOpenedScene;
        if (PossessionManager.Instance.ControlledDemon != null)
            PossessionManager.Instance.ControlledDemon.CanMove = false;
    }

    public IEnumerator SlightFade()
    {
        m_blackPanel.gameObject.SetActive(true);
        float slightFadeAlpha = 0.3f;
        Color nextColor =new Color(Color.black.r, Color.black.g, Color.black.b, slightFadeAlpha);
        Color bloodColor = Color.white;
        bloodColor.a -= slightFadeAlpha;
        float bloodAlphaFadeRate = 1/slightFadeAlpha;
        int selectedSprite = Random.Range(0,bloodSprites.Length);
        bloodSprites[selectedSprite].gameObject.SetActive(true);
        while (slightFadeAlpha > 0)
        {
            bloodColor.a -= Time.unscaledDeltaTime * bloodAlphaFadeRate;
            bloodSprites[selectedSprite].color = bloodColor;
            nextColor.a = slightFadeAlpha;
            m_blackPanel.color = nextColor;
            slightFadeAlpha -= Time.unscaledDeltaTime;
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
        nextColor.a = 0;
        m_blackPanel.color = nextColor;
    }

}
