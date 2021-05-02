using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICollectibles : MonoBehaviour
{
    [SerializeField] Text m_textCount;
    [SerializeField] Sprite[] m_collectiblesSprites;
    [SerializeField] Sprite[] m_unlockedCollectiblesSprites;
    [SerializeField] Sprite lockedCollectibleSprite;
    [SerializeField] Image m_image;
    [SerializeField]
    int m_currentIndex;

    private void Start()
    {
        m_currentIndex = 0;
        Refresh();
    }

    private void OnEnable()
    {
        CalculateCollectibles();
        CameraManager.Instance.HideHotel();
    }

    private void CalculateCollectibles()
    {
        for (int i = 0; i < 6; i++)
        {
            if(PlayerPrefs.GetInt("Hueso."+i) == 1)
            {
                m_collectiblesSprites[i] = m_unlockedCollectiblesSprites[i];
            }
            else
            {
                m_collectiblesSprites[i] = lockedCollectibleSprite;
            }
        }
    }

    public void Next()
    {
        m_currentIndex = (m_currentIndex + 1) % m_collectiblesSprites.Length;
        Refresh();
    }

    public void Previous()
    {
        m_currentIndex--;
        if (m_currentIndex < 0)
            m_currentIndex = m_collectiblesSprites.Length - 1;
        Refresh();
    }

    private void Refresh()
    {
        RefreshImage();
        RefreshText();
    }

    private void RefreshImage()
    {
        m_image.sprite = m_collectiblesSprites[m_currentIndex];
    }

    private void RefreshText()
    {
        m_textCount.text = "" + (m_currentIndex + 1) + " of " + m_collectiblesSprites.Length;
    }

}
