using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Rigidbody2D))]
public class Zarzas : MonoBehaviour
{
    Rigidbody2D m_RGB;
    // Start is called before the first frame update
    void Start()
    {
        m_RGB = GetComponent<Rigidbody2D>();
    }

    public void Explode(Vector3 origin,float force)
    {
        GetComponent<Spikes>().enabled = false;
        GetComponent<Collider2D>().enabled = false;
        m_RGB.isKinematic = false;
        m_RGB.AddForce((transform.position - origin).normalized * force, ForceMode2D.Impulse);
    }
}
