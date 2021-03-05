using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionSaver : MonoBehaviour
{
    [SerializeField] Transform m_upTransform;
    [SerializeField] Transform m_downTransform;
    string m_elevatorPositionKey = "AlmacenElevatorPosition";
    ButtonActivatedDoor m_doorCMP;


    // Start is called before the first frame update
    void Start()
    {
        m_doorCMP = GetComponent<ButtonActivatedDoor>();
        
        //SetPosition();
        //GetComponent<ActivatedBase>().OnActivated += SavePosition;     
        
    }

    private void OnDisable()
    {
        //GetComponent<ActivatedBase>().OnActivated -= SavePosition;
    }
    private void SetPosition()
    {
        if (PlayerPrefs.HasKey(m_elevatorPositionKey))
        {
            if (PlayerPrefs.GetInt(m_elevatorPositionKey) == 0)
            {
                SetPositionDown();
            }
            else if (PlayerPrefs.GetInt(m_elevatorPositionKey) == 1)
            {
                SetPositionUp();
            }
        }
        else
        {
            transform.position = m_downTransform.position;
        }
    }

    public void SetPositionUp()
    {
        //Debug.LogError("Setting position up");
        transform.position = m_upTransform.position;
        if (!m_doorCMP)
            Start();
        m_doorCMP.ChangeEndPosition(m_downTransform.position);
    }

    public void SetPositionDown()
    {
        //Debug.LogError("Setting position down");
        transform.position = m_downTransform.position;
        if (!m_doorCMP)
            Start();
        m_doorCMP.ChangeEndPosition(m_upTransform.position);
    }

    public void SavePosition()
    {
        if (PlayerPrefs.GetInt(m_elevatorPositionKey) == 0)
        {
            PlayerPrefs.SetInt(m_elevatorPositionKey, 1);
        }
        else if (PlayerPrefs.GetInt(m_elevatorPositionKey) == 1)
        {
            PlayerPrefs.SetInt(m_elevatorPositionKey, 0);
        }
    }
}
