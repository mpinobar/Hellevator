using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBeginSatan : MonoBehaviour
{
    [SerializeField] FallEndSatan fallEnd;
    [SerializeField] RuntimeAnimatorController fallController;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out DemonBase character))
        {
            character.GetComponent<Animator>().runtimeAnimatorController = fallController;
            character.GetComponent<Animator>().SetBool("falling", true);
            fallEnd.character = character;
            fallEnd.startXPosition = character.transform.position.x;
        }
    }
}
