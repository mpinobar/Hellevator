using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trituradora : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicZombie character = collision.GetComponentInParent<BasicZombie>();
        if (character)
        {
            character.Die(true);
            character.UnparentLimbs(0);
            //character.gameObject.SetActive(false);
        }
    }
}
