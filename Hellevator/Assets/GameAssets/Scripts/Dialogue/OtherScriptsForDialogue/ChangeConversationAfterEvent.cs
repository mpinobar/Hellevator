using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeConversationAfterEvent : MonoBehaviour
{
    [SerializeField] private TriggerDialogo m_dialogoBeforeKey = null;
    [SerializeField] private TriggerDialogo m_dialogoAfterKey = null;
    [SerializeField] Key key;
    [SerializeField] private GameObject m_canvas = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(PlayerPrefs.GetInt(key.ToString()) == 1)
        {
            m_dialogoAfterKey.StartDialogue(m_canvas);
        }
        else
        {
            m_dialogoBeforeKey.StartDialogue(m_canvas);
        }

        InputManager.Instance.IsInInteactionTrigger = true;
        InputManager.Instance.ResetPlayerHorizontalInput();
        InputManager.Instance.IsInDialogue = true;
    }
}
