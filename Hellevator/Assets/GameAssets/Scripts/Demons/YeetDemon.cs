using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YeetDemon : MonoBehaviour
{
    [SerializeField] Transform m_travelPoint;
    [SerializeField] Vector2 m_yeetVelocity = Vector2.zero;
    Animator m_animator;
    [SerializeField] float m_travelTime = 1f;

    // Start is called before the first frame update
    void Start()
    {
        m_animator = GetComponent<Animator>();
    }

    // Update is called once per frame


    private void OnTriggerEnter2D(Collider2D collision)
    {
        BasicZombie character = collision.GetComponentInParent<BasicZombie>();
        if (character != null && character.IsControlledByPlayer)
        {
            StartCoroutine(Travel(character));
        }
    }

    private IEnumerator Travel(BasicZombie character)
    {
        //Debug.LogError(m_animator.GetBool("Travel"));
        m_animator.SetBool("Travel", true);
        //Debug.LogError(m_animator.GetBool("Travel"));
        character.CanMove = false;
        character.SetJumped();

        Rigidbody2D rgb = character.GetComponent<Rigidbody2D>();
        rgb.gravityScale = 0;
        rgb.isKinematic = true;
        character.transform.eulerAngles = Vector3.zero;
        
        rgb.velocity = Vector2.zero;


        //rgb.freezeRotation = true;
        Collider2D col = m_travelPoint.GetComponent<Collider2D>();
        col.enabled = false;


        float time = 0f;
        while (time <= m_travelTime)
        {
            character.transform.position = m_travelPoint.position;
            time += Time.deltaTime;
            yield return null;
        }

        m_animator.SetBool("Travel", false);

        
        character.CanMove = true;
        
        rgb.isKinematic = false;
        rgb.velocity = m_yeetVelocity;
        character.SetJumped();
        yield return new WaitForSeconds(1.5f);

        col.enabled = true;
        // character.transform.position = m_travelPoint.position;

    }

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    BasicZombie character = collision.GetComponentInParent<BasicZombie>();
    //    if (character != null && character.IsControlledByPlayer)
    //    {
    //        StartCoroutine(Reset());
    //        m_animator.SetBool("Travel", false);
    //        //m_platformCollider.enabled = false;
    //    }
    //}

    IEnumerator Reset()
    {
        Collider2D col = m_travelPoint.GetComponent<Collider2D>();
        col.enabled = false;
        yield return new WaitForSeconds(1.5f);
        col.enabled = true;
    }

}
