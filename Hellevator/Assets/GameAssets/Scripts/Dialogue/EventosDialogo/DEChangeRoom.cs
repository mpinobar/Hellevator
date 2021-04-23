using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEChangeRoom : DialogEvent
{
    [SerializeField] string m_linkedScene;
    public override void ActivateEvent()
    {
        PossessionManager.Instance.ChangeMainCharacter(PossessionManager.Instance.ControlledDemon);
        LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
    }
}
