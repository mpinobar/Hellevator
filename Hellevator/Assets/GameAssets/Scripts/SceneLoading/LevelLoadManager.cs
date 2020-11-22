using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelLoadManager : MonoBehaviour
{
    string m_thisSceneName;
    List<string> m_adjacentScenes;

    public List<string> AdjacentScenes { get => m_adjacentScenes; }
    public string ThisSceneName { get => m_thisSceneName; set => m_thisSceneName = value; }

    private void OnEnable()
    {
        Cursor.visible = false;
        ThisSceneName = gameObject.scene.name;
        LevelManager.Instance.CentralSceneLoadManager = this;
        //if (LevelManager.Instance.CentralScene == null)
        //{
        //    //Debug.LogError("Loading scene as the first one: " + ThisSceneName);
        //    LevelManager.Instance.LoadCentralSceneFirstTime(this);
        //}
        SetControlledCharacterPositionAfterEntering(LevelManager.Instance.PreviousScene);
        
    }

    public void SetControlledCharacterPositionAfterEntering(string previousSceneName)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            string triggerLinkedScene = transform.GetChild(i).GetComponent<TriggerSceneChange>().LinkedScene;
            if (triggerLinkedScene == previousSceneName)
            {
                PossessionManager.Instance.ControlledDemon.gameObject.SetActive(true);
                PossessionManager.Instance.ControlledDemon.transform.position = transform.GetChild(i).GetComponent<TriggerSceneChange>().PositionToSetAfterEntering.position;
            }
        }
    }




    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if(collision.GetComponent<DemonBase>().IsControlledByPlayer)
    //    LevelManager.Instance.ChangeCentralScene(this);
    //}

    //public void SwitchToScene(string newScene)
    //{
    //    LevelManager.Instance.SwitchToAdjacentScene(newScene);
    //}
}
