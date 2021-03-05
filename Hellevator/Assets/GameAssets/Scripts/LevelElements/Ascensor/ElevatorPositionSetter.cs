using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElevatorPositionSetter : MonoBehaviour
{
    [SerializeField] bool m_setsElevatorUp;

    [SerializeField]
    PositionSaver m_elevator;

    TriggerSceneChange m_sceneChangeTrigger;

    // Start is called before the first frame update
    public void ElevatorPositionRefresh(TriggerSceneChange trigger)
    {
        if (m_sceneChangeTrigger == trigger)
        {
            Debug.LogError(trigger.name);
            if (m_setsElevatorUp)
                m_elevator.SetPositionUp();
            else
                m_elevator.SetPositionDown();
        }
    }

    private void Awake()
    {
        m_sceneChangeTrigger = GetComponent<TriggerSceneChange>();
        //TriggerSceneChange.OnPositionSet += ElevatorPositionRefresh;
    }
    //private void OnDisable()
    //{
    //    TriggerSceneChange.OnPositionSet -= ElevatorPositionRefresh;
    //}
}
