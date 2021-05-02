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
    [SerializeField] Satan m_satan;
    Camera m_cam;
    private void OnEnable()
    {
        if (!m_cam)
            m_cam = Camera.main;
        if (!m_colliderCmp)
            m_colliderCmp = GetComponent<Collider2D>();
        m_colliderCmp.enabled = false;
        Enabled = true;
        if (PossessionManager.Instance.ControlledDemon)
        {
            if (m_yeetsOnContact)
                StartCoroutine(HorizontalMovement());
            else
                StartCoroutine(VerticalMovement());
        }
        else
        {
            gameObject.SetActive(false);
            Enabled = false;
        }
    }

    IEnumerator VerticalMovement()
    {
        Vector3 position = m_cam.ViewportToWorldPoint(new Vector3(0,1,0));
        Vector3 end = m_cam.ViewportToWorldPoint(new Vector3(0,0,0));
        float t = 0;
        Vector2 initialPosition = new Vector2(PossessionManager.Instance.ControlledDemon.transform.position.x, position.y - 10);
        //Debug.LogError(initialPosition);
        RaycastHit2D[] hits = Physics2D.RaycastAll(initialPosition,Vector2.down,Mathf.Abs(position.y - end.y));
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.CompareTag("Floor"))
            {
                m_maximumDistance = hits[i].distance;
                break;
            }
        }
        Vector2 endPosition = initialPosition - Vector2.up * m_maximumDistance + Vector2.up*2;
        while (t < 1)
        {
            if (t > 0.7f)
                m_colliderCmp.enabled = true;
            t += Time.deltaTime * animationSpeed;
            transform.position = Vector2.LerpUnclamped(initialPosition, endPosition, m_movementCurve.Evaluate(t));
            yield return null;
        }
        CameraManager.Instance.CameraShakeMedium();
        yield return new WaitForSeconds(m_timeBeforeDisablingCollider);
        m_colliderCmp.enabled = false;
        yield return new WaitForSeconds(1);
        Deactivate();
    }

    IEnumerator HorizontalMovement()
    {
        float t = 0;
        int side;
        if (UnityEngine.Random.value > 0.5f)
            side = 1;
        else
            side = -1;
        //Debug.LogError(side);
        transform.root.localScale = new Vector3(Mathf.Abs(transform.root.localScale.x) * side, transform.root.localScale.y, 1);
        if (side == -1)
            side = 0;
        Vector3 position = m_cam.ViewportToWorldPoint(new Vector3(side,0,0));
        Vector2 initialPosition = new Vector2(position.x, PossessionManager.Instance.ControlledDemon.transform.position.y);
        //Debug.LogError(initialPosition);

        Vector2 endPosition = initialPosition - Vector2.right * m_maximumDistance * (transform.root.localScale.x/Mathf.Abs(transform.root.localScale.x));
        endPosition.x = PossessionManager.Instance.ControlledDemon.transform.position.x;

        while (t < 1)
        {
            if (t < 0.7f)
            {
                position = m_cam.ViewportToWorldPoint(new Vector3(side, 0, 0));
                initialPosition = new Vector2(position.x, initialPosition.y);
                endPosition = initialPosition;
                if (PossessionManager.Instance.ControlledDemon)
                    endPosition.x = PossessionManager.Instance.ControlledDemon.transform.position.x;
                else
                    Deactivate();
            }
            else
            {
                m_colliderCmp.enabled = true;
            }
            t += Time.deltaTime * animationSpeed;
            transform.position = Vector2.LerpUnclamped(initialPosition, endPosition, m_movementCurve.Evaluate(t));
            yield return null;
        }
        CameraManager.Instance.CameraShakeMedium();
        yield return new WaitForSeconds(m_timeBeforeDisablingCollider);
        m_colliderCmp.enabled = false;
        yield return new WaitForSeconds(1);
        Deactivate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Enabled && collision.TryGetComponent(out DemonBase character) && character.IsControlledByPlayer)
        {
            Enabled = false;
            character.Die(true);
            character.CanMove = false;
            if (m_yeetsOnContact)
            {
                character.Torso.GetComponent<Rigidbody2D>().velocity = new Vector2((character.Torso.position - transform.position).normalized.x * m_yeetSpeedHorizontal, m_yeetSpeedVertical);
            }
        }
    }

    void Deactivate()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    public void Damage()
    {
        Debug.LogError("Damaged satan hand");
        m_satan.ReceiveDamage();
    }
}
