using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraInterestPoint : MonoBehaviour
{
    [SerializeField] TriggerType m_triggerType = TriggerType.OnActivatedObject;
    [SerializeField] float m_durationPerObject = 3f;
    [SerializeField] float m_delayToStart = 0f;
    [SerializeField] string m_ID;
    [SerializeField] private ActivatedBase m_triggeringObject;
    [SerializeField] private GameObject [] m_objectsToFollow;
    bool m_hasPlayed = false;
    int m_currentIndex = 0;


    private void Start()
    {
        if (PlayerPrefs.HasKey(m_ID) && PlayerPrefs.GetInt(m_ID) == 1)
        {
            m_hasPlayed = true;
        }
        if (!m_hasPlayed && m_triggerType == TriggerType.OnStart)
        {
            StartCoroutine(FollowObjects(m_delayToStart));
        }
        if(m_triggerType == TriggerType.OnActivatedObject && m_triggeringObject == null)
        {
            Debug.LogError("HACE FALTA ARRASTRAR EL OBJETO QUE HACE TRIGGER AL PUNTO DE INTERES");
        }
    }

    IEnumerator FollowObjects(float delay)
    {
        InputManager.Instance.ReleasePlayerInput();
        m_hasPlayed = true;
        yield return new WaitForSeconds(delay);
        CameraManager.Instance.SetUnlimitedSoftZone(true);
        CameraManager.Instance.ChangeFocusOfMainCameraTo(m_objectsToFollow[0].transform);
        float time = 0f;
        while (m_currentIndex < m_objectsToFollow.Length)
        {
            time += Time.deltaTime;
            if (time >= m_durationPerObject)
            {
                time = 0;
                m_currentIndex++;
                if (m_currentIndex < m_objectsToFollow.Length)
                    CameraManager.Instance.ChangeFocusOfMainCameraTo(m_objectsToFollow[m_currentIndex].transform);
            }
            yield return null;
        }
        CameraManager.Instance.ChangeFocusOfMainCameraTo(PossessionManager.Instance.ControlledDemon.transform);
        PlayerPrefs.SetInt(m_ID, 1);
        InputManager.Instance.RegainPlayerInput();
        yield return new WaitForSeconds(1);
        CameraManager.Instance.SetUnlimitedSoftZone(false);
    }

    public void StartFollowing()
    {
        StartCoroutine(FollowObjects(m_delayToStart));
    }

    private void Update()
    {
        if (!m_hasPlayed && m_triggerType == TriggerType.OnActivatedObject)
        {
            if (m_triggeringObject.Active)
            {
                StartFollowing();
            }
        }
    }
}
