using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivarHijoConTeclaNumerica : MonoBehaviour
{
    GameObject active;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                Debug.LogError("" + i);
                active?.SetActive(false);
                transform.GetChild(i).gameObject.SetActive(true);
                active = transform.GetChild(i).gameObject;
            }
        }   
    }
}
