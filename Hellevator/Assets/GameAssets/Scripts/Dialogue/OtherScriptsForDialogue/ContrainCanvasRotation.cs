using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContrainCanvasRotation : MonoBehaviour
{
    void Update()
    {
        if(transform.root.transform.localScale.x < 0 && transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        if (transform.root.transform.localScale.x > 0 && transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }
}
