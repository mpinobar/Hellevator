using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class EditorPlaymodeTintCycler
{
    private static float cycleTimer = 0.0f;
    static bool goingUp;
    static EditorPlaymodeTintCycler()
    {
        EditorApplication.update += Update;
    }

    static void Update()
    {
        if (!EditorApplication.isPlaying)
        {
            return;
        }

        if (goingUp)
            cycleTimer += Time.deltaTime;
        else
            cycleTimer -= Time.deltaTime;

        if (cycleTimer >= 1.0f)
        {
            goingUp = false;
        }
        if (cycleTimer <= 0.0f)
        {
            goingUp = true;
        }
        float R = cycleTimer;
        float G = 0;
        float B = 0.5f;
        Color c = new Color(R, G, B, 1.0f);

        SettingsHelper.playmodeTint = c;
    }
}
