using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformDemon : MonoBehaviour
{

    [SerializeField] float m_distanceToReady = 30f;
    [SerializeField] float m_speed = 1;
    [SerializeField] Transform m_travelPoint;
    [SerializeField] Collider2D m_platformCollider;

    Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
        m_platformCollider.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        m_animator.SetBool("Ready", Vector2.Distance(transform.position, PossessionManager.Instance.ControlledDemon.transform.position) < m_distanceToReady);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicZombie character = collision.GetComponentInParent<BasicZombie>();
        if (character != null && character.IsControlledByPlayer)
        {
            StartCoroutine(Travel(character));
        }
    }
    IEnumerator Travel(BasicZombie character)
    {
        m_animator.SetBool("Travel", true);
        character.CanMove = false;
        Rigidbody2D rgb = character.GetComponent<Rigidbody2D>();
        rgb.isKinematic = true;
        character.transform.eulerAngles = Vector3.zero;
        rgb.gravityScale = 0;
        rgb.velocity = Vector2.zero;
        rgb.freezeRotation = true;
        character.transform.parent = m_travelPoint;
        character.transform.position = m_travelPoint.position;
        character.transform.eulerAngles = Vector3.zero;
        character.transform.localEulerAngles -= Vector3.forward * 33f;
        yield return new WaitForSeconds(1.5f);
        //Debug.LogError("Finished travel");
        character.transform.eulerAngles = Vector3.zero;
        character.CanMove = true;
        character.transform.parent = null;
        m_platformCollider.enabled = true;
        rgb.isKinematic = false;
        character.ResetJumps();
        //rgb.isKinematic = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        BasicZombie character = collision.GetComponentInParent<BasicZombie>();
        if (character != null && character.IsControlledByPlayer)
        {
            StartCoroutine(Reset());
            m_animator.SetBool("Travel", false);
            m_platformCollider.enabled = false;
        }
    }

    IEnumerator Reset()
    {
        Collider2D col = m_travelPoint.GetComponent<Collider2D>();
        col.enabled = false;
        yield return new WaitForSeconds(.5f);
        col.enabled = true;
    }
}
