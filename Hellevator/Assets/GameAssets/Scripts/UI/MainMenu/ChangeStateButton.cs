using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeStateButton : MonoBehaviour
{

    [SerializeField] MenuCameraState m_nextState = MenuCameraState.Default;

    Button m_button;

    private void Awake()
    {
        m_button = GetComponent<Button>();
        m_button.onClick.AddListener(ChangeState);
    }

    void ChangeState()
    {
        print("hoola");
        FindObjectOfType<MainMenuCanvasController>().ChangeState(m_nextState);
    }

}
