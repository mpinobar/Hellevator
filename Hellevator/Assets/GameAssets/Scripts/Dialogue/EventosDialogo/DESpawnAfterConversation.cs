using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DESpawnAfterConversation : DialogEvent
{
    [SerializeField] private List<GameObject> m_objectToSpawn;

    public override void ActivateEvent()
    {
        for (int i = 0; i < m_objectToSpawn.Count; i++)
        {
            print("");
            m_objectToSpawn[i].SetActive(true);
        }
    }
}
