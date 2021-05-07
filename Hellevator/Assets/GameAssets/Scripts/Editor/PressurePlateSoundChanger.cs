using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
//using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System;
using System.IO;
using UnityEngine.SceneManagement;


public class PressurePlateSoundChanger : EditorWindow
{
    AudioClip clip = null;
    static float offset = 1280f;

    string path;
    string fLetter;
    char firstLetter;

    [MenuItem("Tools/Actualizar sonido de placas de presion")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        //GetWindow(typeof(WorldBabiesUpdater));

        const int width = 800;
        const int height = 400;

        var x = (Screen.currentResolution.width) * 0.5f;
        var y = -Screen.currentResolution.height * 0.5f + offset;

        EditorWindow win = GetWindow(typeof(PressurePlateSoundChanger));
        win.position = new Rect(new Vector2(x * 0.5f, y * 0.5f), new Vector2(width, height));
        //GetWindow<NormalizeZValue>();//.position = new Rect(new Vector2(x, y), new Vector2(width, height)); 
        win.maxSize = new Vector2(600, 500);
        win.minSize = new Vector2(200, 100);

    }


    private void OnGUI()
    {

        GUILayout.Space(20f);
        GUILayout.Label("Nuevo sonido a establecer", EditorStyles.boldLabel);
        GUILayout.Space(5f);

        clip = (AudioClip)EditorGUILayout.ObjectField(clip, typeof(AudioClip), true);
        GUILayout.Space(20f);
        GUILayout.Label("Ruta de la carpeta contenedora de las escenas a cambiar a partir de carpeta LEVEL", EditorStyles.boldLabel);
        GUILayout.Space(5f);
        GUILayout.Label("Ejemplo: ZonaCocinas/Hall", EditorStyles.label);
        GUILayout.Space(5f);
        path = EditorGUILayout.TextField(path);

        GUILayout.Space(20f);
        GUILayout.Label("Primera letra de las escenas a cambiar ", EditorStyles.boldLabel);
        GUILayout.Space(5f);
        fLetter = EditorGUILayout.TextField(fLetter);
        if (fLetter != null && fLetter.Length > 0)
            firstLetter = fLetter[0];


        GUILayout.Space(20f);
        if (GUILayout.Button("Actualizar clip "))
        {
            CargarEscenas();

            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }
    }

    private void CargarEscenas()
    {
        string path = "Assets/GameAssets/Scenes/Level/" + this.path + "/";

        string[] s = Directory.GetFiles(path);


        for (int i = 0; i < s.Length; i++)
        {
            if (Path.GetExtension(s[i]) == ".meta")
            {
                continue;
            }
            else
            {
                if (s[i].Contains(firstLetter + "."))
                {
                    var newScene = EditorSceneManager.OpenScene(s[i], OpenSceneMode.Additive);
                    EditorSceneManager.MarkSceneDirty(newScene);
                    //SceneManager.LoadScene(s[i], LoadSceneMode.Additive);// + "World" + mundo + "." + i);
                    string[] levelnumber = s[i].Split('.');
                    int level = int.Parse(levelnumber[1]);

                    //Debug.Log("Set all gameobjects of scene " + firstLetter + "." + level + " to z = 0");


                    GameObject[] parentGOS = newScene.GetRootGameObjects();

                    ChangeSound(parentGOS);
                    //for (int j = 0; j < parentGOS.Length; j++)
                    //{
                    //    //set all child objects of parentGOS z value to 0
                    //}

                    EditorSceneManager.SaveScene(newScene);
                    EditorSceneManager.CloseScene(newScene, true);
                }
            }
        }
    }

    private void ChangeSound(GameObject[] parentObjs)
    {
        for (int i = 0; i < parentObjs.Length; i++)
        {
            if (parentObjs[i].GetComponent<CameraManager>())
            {
                continue;
            }
            else
            {
                Transform[] children =  parentObjs[i].transform.GetComponentsInChildren<Transform>();

                for (int j = 0; j < children.Length; j++)
                {
                    if(children[j].TryGetComponent(out WeightedPreassurePlate plate))
                    {
                        plate.m_buttonActivatedSFX = clip;
                    }
                }
            }
        }
    }
}


