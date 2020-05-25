using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseButton : MonoBehaviour
{
    [SerializeField] GameObject m_objectToClose;

    public void OnClosePressed()
    {
        m_objectToClose.SetActive(false);
    }
}
