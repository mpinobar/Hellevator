using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualRotation : MonoBehaviour
{    
    [SerializeField] float m_angularSpeed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward * m_angularSpeed * Time.deltaTime);
    }
}
