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
        m_locationImage = GetComponent<Image>();
    }

    public string Location { get => m_location; set => m_location = value; }

    public void ActivateLocation()
    {
        m_locationImage.color = Color.green;
    }
    public void DeactivateLocation()
    {
        m_locationImage.color = Color.white;
    }

}