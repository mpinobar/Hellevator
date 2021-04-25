using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElevatorFade : MonoBehaviour
{
    Animation anim;
    private void Awake()
    {
        anim = GetComponent<Animation>();
    }

    public void WhiteVerticalFade()
    {        
        anim.Play();
    }
}
