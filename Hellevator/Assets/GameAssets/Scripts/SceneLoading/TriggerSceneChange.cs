using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string m_linkedScene;
    [SerializeField] Transform m_positionToSetAfterEntering;

    public string LinkedScene { get => m_linkedScene; set => m_linkedScene = value; }
    public Transform PositionToSetAfterEntering { get => m_positionToSetAfterEntering; set => m_positionToSetAfterEntering = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<DemonBase>().IsControlledByPlayer)
        {
            //Debug.LogError("level load to " + m_linkedScene);
            LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
        }
    }   
}
