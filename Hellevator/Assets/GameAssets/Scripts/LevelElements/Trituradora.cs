﻿using System.Collections;
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
            if (character.GetComponentInParent<SpawnerMatadero>())
            {
                character.GetComponentInParent<SpawnerMatadero>().AttachCharacterPart(character.UnparentLimbs());
            }
            else
                character.UnparentBodyParts(0);
            //character.gameObject.SetActive(false);
        }
    }
}
