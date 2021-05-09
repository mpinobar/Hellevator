using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEStartSatanFight : DialogEvent
{
    [SerializeField] Satan m_satan;
    [SerializeField] GameObject m_botones;

    public override void ActivateEvent()
    {
        m_satan.BeginFight();
        m_botones.SetActive(false);
    }
}