using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEWalk : DialogEvent
{
    [SerializeField] private Transform m_positionToSet = null;

    public override void ActivateEvent()
    {
        StartCoroutine(WalkOnEndTalking());
    }

    private IEnumerator WalkOnEndTalking()
    {
        Debug.LogError("Starting to move character");
        float distance = Mathf.Abs(PossessionManager.Instance.ControlledDemon.transform.position.x - m_positionToSet.position.x);
        PossessionManager.Instance.ControlledDemon.CanMove = false;
        while (distance > 1)
        {
            distance = Mathf.Abs(PossessionManager.Instance.ControlledDemon.transform.position.x - m_positionToSet.position.x);
            PossessionManager.Instance.ControlledDemon.Move((m_positionToSet.position - PossessionManager.Instance.ControlledDemon.transform.position).x / distance);
            print((m_positionToSet.position - PossessionManager.Instance.ControlledDemon.transform.position).x/distance);
            yield return null;
        }
        PossessionManager.Instance.ControlledDemon.CanMove = true;
    }
}
