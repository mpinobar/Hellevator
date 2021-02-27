using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoulSyphon : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character)
        {
            character.IsPossessionBlocked = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character)
        {
            character.IsPossessionBlocked = false;
        }
    }
}
