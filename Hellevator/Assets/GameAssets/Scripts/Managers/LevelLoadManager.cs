using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoadManager : MonoBehaviour
{
    string m_thisSceneName;
    [SerializeField] List<string> m_adjacentScenes;   

    public List<string> AdjacentScenes { get => m_adjacentScenes; }
    public string ThisSceneName { get => m_thisSceneName; set => m_thisSceneName = value; }

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
