using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectScaler : MonoBehaviour
{
    Camera camMain;
    private void OnEnable()
    {
        if (!camMain)
            camMain = Camera.main;
    }

    private void LateUpdate()
    {
        transform.localScale = Vector3.one * camMain.orthographicSize*0.0428f + Vector3.one*0.058f;
    }
}
