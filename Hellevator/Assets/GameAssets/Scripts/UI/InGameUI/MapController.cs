using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{

    [SerializeField] List<MapLocation> m_mapLocations;
    [SerializeField] GameObject m_mapParent;
    private bool m_showingMap;
    public void ShowMap()
    {
        ActivateMap();
        LocatePlayerInMap();
    }

    private void LocatePlayerInMap()
    {
        string playerScene = PossessionManager.Instance.ControlledDemon.gameObject.scene.name;
        for (int i = 0; i < m_mapLocations.Count; i++)
        {
            if(m_mapLocations[i].Location == playerScene)
            {
                m_mapLocations[i].ActivateLocation();
            }
            else
            {
                m_mapLocations[i].DeactivateLocation();

            }
        }
    }

    private void OnEnable()
    {
        UpdateMap();
        ShowMap();
        //if (!m_showingMap)
        //    ShowMap();
        //else
        //    HideMap();
    }

    private void UpdateMap()
    {
        for (int i = 0; i < m_mapLocations.Count; i++)
        {
            //Debug.LogError(m_mapLocations[i].Location + " location has key: " + PlayerPrefs.GetInt(m_mapLocations[i].Location));
            if (PlayerPrefs.HasKey(m_mapLocations[i].Location) && PlayerPrefs.GetInt(m_mapLocations[i].Location) > 0)
            {
                //Debug.LogError("Showing in map zone " + m_mapLocations[i].Location);
                m_mapLocations[i].ShowLocation();
            }
            else
            {
                //Debug.LogError("NOT Showing in map zone " + m_mapLocations[i].Location);
                m_mapLocations[i].HideLocation();
            }
        }
    }

    private void HideMap()
    {
        m_mapParent.SetActive(false);
        m_showingMap = false;
    }

    private void ActivateMap()
    {
        m_showingMap = true;
        m_mapParent.SetActive(true);
    }
}
