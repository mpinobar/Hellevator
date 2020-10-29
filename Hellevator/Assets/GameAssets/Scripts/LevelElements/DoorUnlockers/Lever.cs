using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : ButtonActivatedBase
{
    [SerializeField] ButtonActivatedDoor m_doorToUnlock;

    public override void Activate()
    {
        m_doorToUnlock.Activate();
    }
        
}
