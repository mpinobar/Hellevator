using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cama : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.transform.TryGetComponent(out BasicZombie character))
        {
            if (character.IsControlledByPlayer)
                character.ResetJumps();
        }
    }
}
