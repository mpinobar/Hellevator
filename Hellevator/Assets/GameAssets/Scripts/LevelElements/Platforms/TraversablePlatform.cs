using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraversablePlatform : MonoBehaviour
{
    //Collider2D m_traversableCollider;
    Collider2D m_myCollider;

    private void Start()
    {
        m_myCollider = GetComponent<Collider2D>();
        //m_traversableCollider = transform.GetChild(0).GetComponent<Collider2D>();
    }

    public void Traverse()
    {        
        m_myCollider.enabled = false;
        StartCoroutine(ReactivateCollider());
    }

    IEnumerator ReactivateCollider()
    {
        yield return new WaitForSeconds(0.35f);
        m_myCollider.enabled = true;
    }

}
