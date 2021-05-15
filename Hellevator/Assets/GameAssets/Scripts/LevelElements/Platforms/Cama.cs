using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cama : MonoBehaviour
{
    [SerializeField] float m_jumpBounciness;
    [SerializeField] float m_normalBounciness;

    [SerializeField] AudioClip bedSound;

    public void SetBouncy(BasicZombie character)
    {
        character.ResetJumps();
        character.MyRgb.velocity = new Vector2(character.MyRgb.velocity.x, 0);
        character.MyRgb.AddForce(Vector2.up * (character).JumpForce * m_jumpBounciness);
        AudioManager.Instance.PlayAudioSFX(bedSound, false);
    }


}
