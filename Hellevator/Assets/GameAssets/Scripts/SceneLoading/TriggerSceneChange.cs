using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TriggerSceneChange : MonoBehaviour
{
    [SerializeField] string m_linkedScene;
    [SerializeField] Transform m_positionToSetAfterEntering;

    [SerializeField] AudioClip m_changeSceneSFX = null;
    [SerializeField] float m_delayToActivateCollider = 0f;
    [SerializeField] bool m_stopsMusic = false;
    public string LinkedScene { get => m_linkedScene; set => m_linkedScene = value; }
    public Transform PositionToSetAfterEntering
    {
        get
        {
            if (TryGetComponent(out ElevatorPositionSetter setter))
            {                
                setter.ElevatorPositionRefresh(this);
            }
            if (m_stopsMusic)
                AudioManager.Instance.StopMusic();
            isChangingRoom = false;
            return m_positionToSetAfterEntering;
        }
        set
        {
            m_positionToSetAfterEntering = value;

        }
    }

    //public static Action<TriggerSceneChange> OnPositionSet;

    private IEnumerator Start()
    {
        if (m_delayToActivateCollider > 0)
        {
            GetComponent<Collider2D>().enabled = false;
            yield return new WaitForSeconds(m_delayToActivateCollider);
            GetComponent<Collider2D>().enabled = true;
        }

    }
    static bool isChangingRoom;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase demon = collision.GetComponent<DemonBase>();
        if (demon != null)
        {
            if (demon.IsControlledByPlayer && !isChangingRoom)
            {
                //Debug.LogError(demon.name + " entered trigger "+name);
                //Debug.LogError("Loading scene " + m_linkedScene + " from " + gameObject.scene.name);
                isChangingRoom = true;
                AudioManager.Instance.PlayAudioSFX(m_changeSceneSFX, false, 2f);
                PossessionManager.Instance.ChangeMainCharacter(demon);
                LevelManager.Instance.SwitchToAdjacentScene(m_linkedScene);
                GetComponent<Collider2D>().enabled = false;
                InputManager.Instance.IsInInteactionTrigger = false;
                InputManager.Instance.IsInDialogue = false;
                GC.Collect();
                //Debug.LogError("Entered trigger to switch scenes to " + m_linkedScene);
            }
        }
    }
}
