using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndChaseStopper : MonoBehaviour
{
    [SerializeField] PersecucionFinal plataformaPersecucion;
    [SerializeField] ButtonActivatedDoor door;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
            {
                plataformaPersecucion.Deactivate();
                door.Activate();
            }
        }
    }
}
