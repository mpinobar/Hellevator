using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AscensorHabitaciones : MonoBehaviour
{
    [SerializeField] GameObject[] m_buttons;
    [SerializeField] string [] m_buttonsToCheck;
    [SerializeField] string [] m_activatedButtons;
    [SerializeField] string [] m_scenesToGoTo;
    [SerializeField] Transform m_positionToSet;
    bool m_shouldShowInitialAnimation;
    [SerializeField] TriggerSceneChange m_leftSceneChange;
    [SerializeField] TriggerSceneChange m_rightSceneChange;
    [SerializeField] ElevatorFade m_elevatorFade;
    [SerializeField] Transform m_leftDoor;
    [SerializeField] Transform m_rightDoor;
    private void Awake()
    {
        m_shouldShowInitialAnimation = PlayerPrefs.GetInt("ElevatorAnim") == 0;
        ActivateButtons();
    }

    private void Start()
    {
        if (m_shouldShowInitialAnimation)
        {
            ShowInitialAnimation();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DemonBase character))
        {
            if (character.IsControlledByPlayer)
            {
                //if (m_shouldShowInitialAnimation)
                //{
                //    ShowInitialAnimation();
                //    return;
                //}

                ActivateButtons();

                CheckNewButtonsToActivate();
            }
        }
    }

    private void CheckNewButtonsToActivate()
    {
        for (int i = 0; i < m_buttonsToCheck.Length; i++)
        {
            if (PlayerPrefs.GetInt(m_buttonsToCheck[i]) == 1 && PlayerPrefs.GetInt(m_activatedButtons[i]) == 0)
            {
                PlayerPrefs.SetInt(m_activatedButtons[i], 1);
                //Debug.LogError("New button to activate" + i);
                StartCoroutine(AnimateNewButtonActivated(i));
            }
        }
    }

    private IEnumerator AnimateNewButtonActivated(int button)
    {
        //Debug.LogError("Starting to move character");
        float distance = Mathf.Abs(PossessionManager.Instance.ControlledDemon.transform.position.x - m_positionToSet.position.x);
        InputManager.Instance.ResetPlayerHorizontalInput();
        PossessionManager.Instance.ControlledDemon.CanMove = false;
        while (distance > 1)
        {
            distance = Mathf.Abs(PossessionManager.Instance.ControlledDemon.transform.position.x - m_positionToSet.position.x);
            PossessionManager.Instance.ControlledDemon.MyRgb.velocity = new Vector3(((m_positionToSet.position - PossessionManager.Instance.ControlledDemon.transform.position).x * ((BasicZombie)PossessionManager.Instance.ControlledDemon).MaxSpeed / distance), PossessionManager.Instance.ControlledDemon.MyRgb.velocity.y, 0);
            yield return null;
        }
        //Debug.LogError("Finished moving character to position");
        ((BasicZombie)PossessionManager.Instance.ControlledDemon).SetOnLadder(true);
        yield return new WaitForSeconds(1);
        m_buttons[button].SetActive(true);
        //Debug.LogError("Set button");
        yield return new WaitForSeconds(1);
        ((BasicZombie)PossessionManager.Instance.ControlledDemon).SetOnLadder(false);
        AnimateElevatorMovement(button);
    }

    void AnimateElevatorMovement(int sceneIndex)
    {        
        CloseElevatorDoors();
        PossessionManager.Instance.ControlledDemon.CanMove = false;
        SetSceneChangers(sceneIndex);
        StartCoroutine(ShakeAndFade());

    }

    private void SetSceneChangers(int sceneIndex)
    {
        //CloseElevatorDoors();
        switch (sceneIndex)
        {
            case -1:
                {
                    m_rightDoor.gameObject.SetActive(false);
                    break;
                }
            case 0:
                {
                    m_leftSceneChange.LinkedScene = m_scenesToGoTo[sceneIndex];
                    StartCoroutine(DelayOpenDoor(m_leftDoor));
                    break;
                }
            case 1:
                {
                    m_rightSceneChange.LinkedScene = m_scenesToGoTo[sceneIndex];
                    StartCoroutine(DelayOpenDoor(m_rightDoor));
                    break;
                }
            case 2:
                {
                    m_leftSceneChange.LinkedScene = m_scenesToGoTo[sceneIndex];
                    StartCoroutine(DelayOpenDoor(m_leftDoor));
                    break;
                }
            default:
                break;
        }
    }

    private IEnumerator DelayOpenDoor(Transform doorToOpen)
    {
        yield return new WaitForSeconds(3);
        doorToOpen.gameObject.SetActive(false);

    }

    private void CloseElevatorDoors()
    {
        m_leftDoor.gameObject.SetActive(true);
        m_rightDoor.gameObject.SetActive(true);

    }

    IEnumerator ShakeAndFade()
    {
        //Debug.LogError("Animating elevator movement ");
        PossessionManager.Instance.ControlledDemon.CanMove = false;
        m_elevatorFade.gameObject.SetActive(true);
        CameraManager.Instance.CameraShakeLight3S();
        yield return new WaitForSeconds(3);
        PossessionManager.Instance.ControlledDemon.CanMove = true;
        m_elevatorFade.gameObject.SetActive(false);
    }
    private void ActivateButtons()
    {
        int min = -1;
        for (int i = 0; i < m_activatedButtons.Length; i++)
        {
            if (PlayerPrefs.GetInt(m_activatedButtons[i]) == 1)
            {
                min = i;
                //Debug.LogError("Activating button" + i);
                m_buttons[i].SetActive(true);
                if(i == 0 || i == 2)
                {
                    CloseElevatorDoors();
                    m_leftDoor.gameObject.SetActive(false);
                }else if (i == 1)
                {
                    CloseElevatorDoors();
                    m_rightDoor.gameObject.SetActive(false);
                }                
            }
        }
        //Debug.LogError(min);
        SetSceneChangers(min);
    }

    private void ShowInitialAnimation()
    {        
        PlayerPrefs.SetInt("ElevatorAnim", 1);
        //Debug.LogError("init anim");
        m_shouldShowInitialAnimation = false;
        AnimateElevatorMovement(-1);
    }
}
