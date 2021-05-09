using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallEndSatan : MonoBehaviour
{
    public DemonBase character;
    public float startXPosition;

    private void LateUpdate()
    {
        if (character)
        {
            character.transform.position = new Vector3(startXPosition, character.transform.position.y, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DemonBase character) && character.IsControlledByPlayer)
        {
            character.GetComponent<Animator>().SetBool("falling", false);
            this.character = null;
        }
    }
}
