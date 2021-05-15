using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectScaler : MonoBehaviour
{
    Camera camMain;
    [SerializeField] public float scaling = 0.155f;
    private void OnEnable()
    {
        if (!camMain)
            camMain = Camera.main;
    }

    private void LateUpdate()
    {
        transform.localScale = Vector3.one * camMain.orthographicSize*scaling /*+ Vector3.one*0.05f*/;
    }
}
