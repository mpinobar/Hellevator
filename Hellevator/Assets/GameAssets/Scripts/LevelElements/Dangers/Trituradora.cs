using System.Collections;
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
			AudioManager.Instance.PlayAudioSFX(m_sawingClip, false);
            if (!character.IsDead)
            {
                character.Die(true);
            }else
            {
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
