using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class DEAfterFinalConversation : DialogEvent
{
    [SerializeField] private CinemachineVirtualCamera m_camera;
    [SerializeField] private DestructibleWall m_trapDoor;
    [SerializeField] private Transform m_fall;
    [SerializeField] private Transform m_auxTrans;
    [SerializeField] RuntimeAnimatorController animatorController;
    public override void ActivateEvent()
    {
        m_fall.SetParent(m_auxTrans, true);
        m_camera.Priority = 2;
        m_trapDoor.Explode(PossessionManager.Instance.ControlledDemon.transform.position, 80);
        PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().runtimeAnimatorController = animatorController;
        PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().SetBool("falling", true);
        PossessionManager.Instance.ControlledDemon.CanMove = true;
        InputManager.Instance.IsInInteactionTrigger = true;
        DialogueManager.Instance.DeactivateTextAndImage();

        for (int i = 0; i < m_trapDoor.transform.childCount; i++)
        {

            if(m_trapDoor.transform.GetChild(i).GetComponent<Collider2D>() != null)
            {
                m_trapDoor.transform.GetChild(i).GetComponent<Collider2D>().enabled = false;
            }
        }
    }
}
