﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trituradora : MonoBehaviour
{
    [SerializeField] AudioClip m_sawingClip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicZombie character = collision.GetComponentInParent<BasicZombie>();
        if (character)
        {            
            if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
                return;
            
            if (!character.IsDead)
            {
                AudioManager.Instance.PlayAudioSFX(m_sawingClip, false);
                character.Die(true);
            }
            else if (character.isActiveAndEnabled)
            {
                AudioManager.Instance.PlayAudioSFX(m_sawingClip, false);
                character.PlayDeathEffects();
            }
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
