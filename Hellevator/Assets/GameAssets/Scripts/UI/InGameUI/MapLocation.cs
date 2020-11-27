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

    public string Location { get => m_location; set => m_location = value; }
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

}