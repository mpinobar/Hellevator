using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PossessionManager : PersistentSingleton<PossessionManager>
{
    private DemonBase m_controlledDemon;
    public DemonBase ControlledDemon
    {
        get => m_controlledDemon; set => m_controlledDemon = value;
    }
    public PossessingLight PLight
    {
        get => m_pLight; set => m_pLight = value;
    }
    public LayerMask RagdollBodyMask { get => m_ragdollBodyMask; }
    public bool ControllingMultipleDemons { get => controllingMultipleDemons; }

    [SerializeField] LayerMask m_ragdollBodyMask;
    [SerializeField] GameObject m_PossessionLight;
    private PossessingLight m_pLight;

    List<DemonBase> extraDemonsControlled;

    bool controllingMultipleDemons;
   

    private void Start()
    {
        InputManager.Instance.UpdateDemonReference();
        
    }

    /// <summary>
    /// Returns the nearest demon to the demon currently controlled by the player, with a distance limit
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Transform of the currently possessed demon</param>
    /// <returns>Reference to the found demon</returns>
    public DemonBase LookForNearestDemon(float radiusLimit, Transform currentDemon)
    {
        int lookForRadius = 1;

        while (lookForRadius <= radiusLimit)
        {
            Collider2D[] other = Physics2D.OverlapCircleAll(currentDemon.transform.position, lookForRadius, m_ragdollBodyMask);
            Debug.LogError(other.Length);
            for (int i = 0; i < other.Length; i++)
            {
                DemonBase foundDemon = other[i].GetComponentInParent<DemonBase>();

                if (foundDemon != null && foundDemon != ControlledDemon)
                {
                    if (!foundDemon.IsInDanger && foundDemon.IsDead && !foundDemon.IsPossessionBlocked)
                    {
                        return foundDemon;
                    }
                }
            }
            lookForRadius++;
        }
        return null;
    }


    public void RemoveDemonPossession(Transform currentDemon)
    {
        DemonBase demonCmp = currentDemon.GetComponentInParent<DemonBase>();

        if (ControlledDemon == demonCmp)
        {
            if (extraDemonsControlled == null || extraDemonsControlled.Count == 0)
            {
                ControlledDemon.SetNotControlledByPlayer();
                PossessNearestDemon(demonCmp.MaximumPossessionRange, demonCmp);
            }
            else
            {
                ControlledDemon.SetNotControlledByPlayer();
                ControlledDemon = extraDemonsControlled[Random.Range(0, extraDemonsControlled.Count)];
                extraDemonsControlled.Remove(ControlledDemon);

                InputManager.Instance.RemoveExtraDemonControlled(ControlledDemon);
                InputManager.Instance.UpdateDemonReference();
            }
        }
        else
        {
            if (extraDemonsControlled.Contains(demonCmp))
            {
                extraDemonsControlled.Remove(demonCmp);
                InputManager.Instance.RemoveExtraDemonControlled(demonCmp);
                demonCmp.SetNotControlledByPlayer();
                if (extraDemonsControlled.Count == 0)
                {
                    controllingMultipleDemons = false;
                }
            }
        }
    }

    public void PossessAllDemonsInRange(float radiusLimit, Transform currentDemon)
    {
        if (!controllingMultipleDemons)
        {
            if (extraDemonsControlled == null)
            {
                extraDemonsControlled = new List<DemonBase>();
            }
            extraDemonsControlled.Clear();
            Collider2D[] other = Physics2D.OverlapCircleAll(currentDemon.transform.position, radiusLimit, m_ragdollBodyMask);
            for (int i = 0; i < other.Length; i++)
            {
                DemonBase foundDemon = other[i].GetComponentInParent<DemonBase>();

                if (foundDemon != null && foundDemon != ControlledDemon && !extraDemonsControlled.Contains(foundDemon))
                {
                    if (!foundDemon.IsInDanger && foundDemon.IsDead && !foundDemon.IsPossessionBlocked)
                    {
                        extraDemonsControlled.Add(foundDemon);
                        foundDemon.SetControlledByPlayer();
                    }
                }
            }
            if (extraDemonsControlled.Count > 0)
            {
                controllingMultipleDemons = true;
                InputManager.Instance.UpdateExtraDemonsControlled(extraDemonsControlled);
            }
        }
    }

    /// <summary>
    /// Returns the nearest demon to the demon currently controlled by the player, with a distance limit
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Transform of the currently possessed demon</param>
    /// <param name="excluding">A demon to filter out of the search</param>
    /// <returns>Reference to the found demon</returns>
    public DemonBase LookForNearestDemon(float radiusLimit, Transform currentDemon, DemonBase excluding)
    {
        int lookForRadius = 1;

        while (lookForRadius <= radiusLimit)
        {
            Collider2D[] other = Physics2D.OverlapCircleAll(currentDemon.transform.position, lookForRadius, m_ragdollBodyMask);
            for (int i = 0; i < other.Length; i++)
            {
                DemonBase foundDemon = other[i].transform.root.GetComponent<DemonBase>();

                if (foundDemon != null && foundDemon != ControlledDemon && foundDemon != excluding)
                {
                    if (!foundDemon.IsInDanger && foundDemon.IsDead && !foundDemon.IsPossessionBlocked)
                    {
                        return foundDemon;
                    }
                }
            }
            lookForRadius++;
        }
        return null;
    }


    /// <summary>
    /// Switches the player controls to a new demon
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Currently possessed demon</param>
    public void PossessNearestDemon(float radiusLimit, DemonBase currentDemon)
    {

        DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon.transform);

        ControlledDemon = null;
        InputManager.Instance.UpdateDemonReference();

        if (demonToPossess != null)
        {
            if (m_pLight == null)
            {
                m_pLight = Instantiate(m_PossessionLight, currentDemon.transform.position, Quaternion.identity).GetComponent<PossessingLight>();
            }

            m_pLight.gameObject.SetActive(true);
            m_pLight.transform.position = currentDemon.transform.position;
            m_pLight.Begin(demonToPossess, currentDemon.MaximumPossessionRange, currentDemon);

            CameraManager.Instance.ChangeFocusOfMainCameraTo(m_pLight.transform);
            if (CameraManager.Instance.CurrentCamera == CameraManager.Instance.PlayerCamera)
            {
            }
            //CameraManager.Instance.FollowGhost(m_pLight.transform);
        }
        else
        {
            LevelManager.Instance.StartRestartingLevel();
        }
    }

    public void PossessNewDemon(DemonBase demonToPossess)
    {
        demonToPossess.enabled = true;
        demonToPossess.transform.parent = null;
        ControlledDemon = demonToPossess;
        demonToPossess.SetControlledByPlayer();

        //CameraManager.Instance.ChangeCamTarget();
        InputManager.Instance.UpdateDemonReference();
        if (m_pLight != null)
        {
            m_pLight.gameObject.SetActive(false);
        }
    }
}
