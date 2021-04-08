using UnityEngine;
using System;

public class HandAttackSatan : MonoBehaviour
{
    [SerializeField] bool m_yeetsOnContact;
    [SerializeField] float m_yeetSpeedHorizontal = 50f;
    [SerializeField] float m_yeetSpeedVertical = 20f;
    private bool m_enabled;

    public bool Enabled { get => m_enabled; set => m_enabled = value; }


    private void Start()
    {
        Enabled = true;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Enabled && collision.TryGetComponent(out DemonBase character) && character.IsControlledByPlayer)
        {
            character.Die(true);
            character.CanMove = false;
            if (m_yeetsOnContact)
            {
                character.Torso.GetComponent<Rigidbody2D>().velocity = new Vector2((character.Torso.position - transform.position).normalized.x * m_yeetSpeedHorizontal, m_yeetSpeedVertical);
            }
        }
    }
}
