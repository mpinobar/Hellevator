using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadManager : MonoBehaviour
{
    string thisSceneName;
    [SerializeField] List<string> adjacentScenes;   

    public List<string> AdjacentScenes { get => adjacentScenes; }
    public string ThisSceneName { get => thisSceneName; set => thisSceneName = value; }

    private void OnEnable()
    {
        ThisSceneName = gameObject.scene.name;
        if(LevelManager.Instance.CentralScene == null)
        {
            LevelManager.Instance.LoadCentralSceneFirstTime(this);
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        LevelManager.Instance.ChangeCentralScene(this);
    }
}
