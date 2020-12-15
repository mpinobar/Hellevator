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
        //Cursor.visible = false;
        ThisSceneName = gameObject.scene.name;
        LevelManager.Instance.CentralSceneLoadManager = this;
        UIController.Instance.TryDiscoverNewZone(ThisSceneName);
        //if (LevelManager.Instance.CentralScene == null)
        //{
        //    //Debug.LogError("Loading scene as the first one: " + ThisSceneName);
        //    LevelManager.Instance.LoadCentralSceneFirstTime(this);
        //}
        if (LevelManager.Instance.NewSceneName != null && LevelManager.Instance.NewSceneName != "" && LevelManager.Instance.NewSceneName.Split('_') != null && LevelManager.Instance.NewSceneName.Split('_').Length > 1)
        {
           // Debug.LogError("1: "+ int.Parse(LevelManager.Instance.NewSceneName.Split('_')[1]));

            SetControlledCharacterPositionAfterEntering(LevelManager.Instance.PreviousScene, int.Parse(LevelManager.Instance.NewSceneName.Split('_')[1]));
        }
        else
        {
            //Debug.LogError("2");
            SetControlledCharacterPositionAfterEntering(LevelManager.Instance.PreviousScene);
        }
        MusicManager.Instance.CheckMusic();
    }

    public void SetControlledCharacterPositionAfterEntering(string previousSceneName)
    {
        //Debug.LogError("Previous: " + previousSceneName);
        //LevelManager.Instance.PrintSceneName();
        for (int i = 0; i < transform.childCount; i++)
        {
            string triggerLinkedScene = transform.GetChild(i).GetComponent<TriggerSceneChange>().LinkedScene;
            if (triggerLinkedScene == previousSceneName && PossessionManager.Instance.ControlledDemon)
            {
                PossessionManager.Instance.ControlledDemon.gameObject.SetActive(true);
                PossessionManager.Instance.ControlledDemon.transform.position = transform.GetChild(i).GetComponent<TriggerSceneChange>().PositionToSetAfterEntering.position;
                ((BasicZombie)PossessionManager.Instance.ControlledDemon).SetOnLadder(false);
                ((BasicZombie)PossessionManager.Instance.ControlledDemon).MyRgb.velocity = Vector2.zero;
                InputManager.Instance.ResetPlayerInput();
            }
        }
    }

    public void SetControlledCharacterPositionAfterEntering(string previousSceneName, int id)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            string triggerLinkedScene = transform.GetChild(i).GetComponent<TriggerSceneChange>().LinkedScene;

        if (triggerLinkedScene.Split('_')[0] == previousSceneName && PossessionManager.Instance.ControlledDemon && id == int.Parse(triggerLinkedScene.Split('_')[1]))
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
