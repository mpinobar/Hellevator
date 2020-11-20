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


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M)){
            if (!m_showingMap)
                ShowMap();
            else
                HideMap();            
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
