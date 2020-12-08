using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapLocation : MonoBehaviour
{
    [SerializeField] string m_location;
    Image m_locationImage;

    private void Awake()
    {
        LocationImage = GetComponent<Image>();
    }

    public string Location { get => m_location;}
    public Image LocationImage
    {
        get
        {
            if (m_locationImage == null)
                m_locationImage = GetComponent<Image>();

            return m_locationImage;
        }
        set => m_locationImage = value;
    }


    public void ActivateLocation()
    {
        LocationImage.color = Color.green;
    }
    public void DeactivateLocation()
    {

        LocationImage.color = Color.white;
    }
    public void HideLocation()
    {
        LocationImage.enabled = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(false);
        }
    }
    public void ShowLocation()
    {
        LocationImage.enabled = true;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(true);
        }
    }
}