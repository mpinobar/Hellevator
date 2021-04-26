using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DerrumbarSuelo : MonoBehaviour
{
    private void OnEnable()
    {
        Satan.OnInterphase += Animate;
    }

    private void Animate()
    {
        CameraManager.Instance.CameraShakeMega();
        GetComponent<Animation>().Play();
        Satan.OnInterphase -= Animate;
    }

    private void OnDisable()
    {
        Satan.OnInterphase -= Animate;
    }

}
