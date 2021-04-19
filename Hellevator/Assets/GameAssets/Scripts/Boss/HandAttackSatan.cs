using UnityEngine;
using System;
using System.Collections;

public class HandAttackSatan : MonoBehaviour
{
    [SerializeField] float animationSpeed;
    [SerializeField] AnimationCurve m_movementCurve;
    [SerializeField] bool m_yeetsOnContact;
    [SerializeField] float m_yeetSpeedHorizontal = 50f;
    [SerializeField] float m_yeetSpeedVertical = 20f;
    [SerializeField] float m_maximumDistance = 20f;
    [SerializeField] float m_timeBeforeDisablingCollider = 0.5f;
    private bool m_enabled;
    Collider2D m_colliderCmp;
    public bool Enabled { get => m_enabled; set => m_enabled = value; }


    private void OnEnable()
    {
        if (!m_colliderCmp)
            m_colliderCmp = GetComponent<Collider2D>();
        m_colliderCmp.enabled = true;
        Enabled = true;
        if (m_yeetsOnContact)
            StartCoroutine(HorizontalMovement());
        else
            StartCoroutine(VerticalMovement());
    }

    IEnumerator VerticalMovement()
    {
        float t = 0;
        Vector2 initialPosition = transform.position;
        Vector2 endPosition = transform.position - Vector3.up * m_maximumDistance;
        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            transform.position = Vector2.LerpUnclamped(initialPosition, endPosition, m_movementCurve.Evaluate(t));
            yield return null;
        }
        yield return new WaitForSeconds(m_timeBeforeDisablingCollider);
        m_colliderCmp.enabled = false;
    }

    IEnumerator HorizontalMovement()
    {
        float t = 0;
        Vector2 initialPosition = transform.position;
        Vector2 endPosition = transform.position - Vector3.right * m_maximumDistance * transform.root.localScale.x;
        while (t < 1)
        {
            t += Time.deltaTime * animationSpeed;
            transform.position = Vector2.LerpUnclamped(initialPosition, endPosition, m_movementCurve.Evaluate(t));
            yield return null;
        }
        yield return new WaitForSeconds(m_timeBeforeDisablingCollider);
        m_colliderCmp.enabled = false;
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
