using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BelcebuKey : MonoBehaviour
{
    [SerializeField] private string m_linkedScene;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.TryGetComponent<DemonBase>(out DemonBase demon))
        {
            if (demon == PossessionManager.Instance.ControlledDemon)
            {
                PossessionManager.Instance.ChangeMainCharacter(PossessionManager.Instance.ControlledDemon);
                LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
            }
        }
    }
}
