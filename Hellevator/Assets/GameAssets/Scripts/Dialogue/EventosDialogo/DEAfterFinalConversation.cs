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

    public override void ActivateEvent()
    {
        m_fall.SetParent(m_auxTrans, true);
        m_camera.Priority = 2;
        m_trapDoor.Explode(PossessionManager.Instance.ControlledDemon.transform.position, 8);
        PossessionManager.Instance.ControlledDemon.GetComponent<Animator>().SetTrigger("Fall");

    }
}
