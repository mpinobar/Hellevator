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
    public DemonBase DemonShowingSkull { get => demonShowingSkull; set => demonShowingSkull = value; }
    public Boss Boss { get => boss; set => boss = value; }

    [SerializeField] LayerMask m_ragdollBodyMask = 1<<8;
    [SerializeField] GameObject m_PossessionLight;
    private PossessingLight m_pLight;

    List<DemonBase> extraDemonsControlled;

    bool controllingMultipleDemons;

    DemonBase demonShowingSkull;

    [SerializeField] int m_maxDemonsPossessed = 2;

    Boss boss;

    private void Start()
    {
        InputManager.Instance.UpdateDemonReference();

        if (m_PossessionLight == null)
        {
            Debug.LogError("FALTABA POSSESSION MANAGER, CREANDO UNO CON REFERENCIAS POR CODIGO. PARA LA PROXIMA INTENTAD ARRASTRAR UNO A LA ESCENA PARA ALIGERAR LA CARGA DE RECURSOS PLEASE");
            string path = "PossessingLight";
            m_PossessionLight = (GameObject) Resources.Load(path, typeof(GameObject));
            //GameObject go = Instantiate(Resources.Load(path,typeof(GameObject))) as GameObject;
            //PLight = go.GetComponent<PossessingLight>();
        }

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


    public void MoveDemonsToCentralScene(Scene centralScene)
    {
        if (ControlledDemon.transform.parent == null)
        {
            SceneManager.MoveGameObjectToScene(ControlledDemon.gameObject, centralScene);
        }
        else
        {
            Transform parent = ControlledDemon.transform.parent;
            ControlledDemon.transform.parent = null;
            SceneManager.MoveGameObjectToScene(ControlledDemon.gameObject, centralScene);
            ControlledDemon.transform.parent = parent;
        }

        if (extraDemonsControlled != null && extraDemonsControlled.Count > 0)
        {
            for (int i = 0; i < extraDemonsControlled.Count; i++)
            {
                SceneManager.MoveGameObjectToScene(extraDemonsControlled[i].gameObject, centralScene);
            }
        }
    }

    public void MoveMainCharacterToScene(Scene newScene)
    {
        ControlledDemon.transform.parent = null;
        SceneManager.MoveGameObjectToScene(ControlledDemon.gameObject, newScene);
        //if (ControlledDemon.transform.parent == null)
        //{
        //    SceneManager.MoveGameObjectToScene(ControlledDemon.gameObject, newScene);
        //}
        //else
        //{
        //    Transform parent = ControlledDemon.transform.parent;
        //    ControlledDemon.transform.parent = null;
        //    SceneManager.MoveGameObjectToScene(ControlledDemon.gameObject, newScene);
        //    ControlledDemon.transform.parent = parent;
        //}
    }
    public void RemoveDemonPossession(Transform currentDemon)
    {
        if (boss)
        {
            boss.ResetTimer();
        }
        DemonBase demonCmp = currentDemon.GetComponentInParent<DemonBase>();
        //Debug.LogError("possessed character died: " + currentDemon.name);
        if (ControlledDemon == demonCmp)
        {
            //Debug.LogError("it was the main character");
            if (extraDemonsControlled == null || extraDemonsControlled.Count == 0)
            {
                //Debug.LogError("no extra characters controlled");
                if (ControlledDemon)
                    ControlledDemon.SetNotControlledByPlayer();
                if (!currentDemon.GetComponent<DemonBase>().MultiplePossessionWhenDead)
                {
                    //Debug.LogError("should not possess multiple characters on death");
                    PossessNearestDemon(demonCmp.MaximumPossessionRange, demonCmp);
                }
                else
                {
                    //Debug.LogError("SHOULD possess multiple characters on death");
                    ControlledDemon = null;
                }
            }
            else
            {
                //Debug.LogError("extra characters are still alive");
                ControlledDemon.SetNotControlledByPlayer();
                ControlledDemon = extraDemonsControlled[Random.Range(0, extraDemonsControlled.Count)];
                extraDemonsControlled.Remove(ControlledDemon);

                InputManager.Instance.RemoveExtraDemonControlled(ControlledDemon);
                InputManager.Instance.UpdateDemonReference();
                if (extraDemonsControlled.Count == 0)
                {
                    //Debug.LogError("last one of the extra characters died");
                    controllingMultipleDemons = false;
                }
            }
        }
        else
        {
            //Debug.LogError("extra character died. Main demon is " + ControlledDemon);
            if (extraDemonsControlled.Contains(demonCmp))
            {
                extraDemonsControlled.Remove(demonCmp);
                InputManager.Instance.RemoveExtraDemonControlled(demonCmp);
                demonCmp.SetNotControlledByPlayer();
                if (extraDemonsControlled.Count == 0)
                {
                    //Debug.LogError("last one of the extra characters died");
                    controllingMultipleDemons = false;
                }
            }
        }
    }

    public void PossessAllDemonsInRange(float radiusLimit, Transform currentDemon)
    {
        //Debug.LogError("Controlling demons in range");
        int counter = 0;
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
                //Debug.LogError("Candidate " + foundDemon.name);
                //if ((foundDemon.GetComponent<DemonBase>() == ControlledDemon))
                //    Debug.LogError("Is not main demon: " + (foundDemon.GetComponent<DemonBase>() != ControlledDemon));
                //if (extraDemonsControlled.Contains(foundDemon))
                //    Debug.LogError("Isnt already in extra controlled: " + !extraDemonsControlled.Contains(foundDemon));
                //if ((currentDemon == foundDemon.transform))
                //    Debug.LogError("Is not self: " + (currentDemon != foundDemon.transform));
                //if (foundDemon.IsInDanger)
                //    Debug.LogError("Is not in danger: " + !foundDemon.IsInDanger);
                //if (!foundDemon.IsDead)
                //    Debug.LogError("Is dead: " + foundDemon.IsDead);
                //if (foundDemon.IsPossessionBlocked)
                //    Debug.LogError("Has possession available: " + !foundDemon.IsPossessionBlocked);
                //if (!(counter < m_maxDemonsPossessed))
                //    Debug.LogError("Is within limits of max number of possessed: " + (counter < m_maxDemonsPossessed));
                if (foundDemon != null && foundDemon != ControlledDemon && !extraDemonsControlled.Contains(foundDemon) && currentDemon != foundDemon.transform)
                {
                    if (!foundDemon.IsInDanger && foundDemon.IsDead && !foundDemon.IsPossessionBlocked && counter < m_maxDemonsPossessed)
                    {
                        counter++;
                        extraDemonsControlled.Add(foundDemon);
                        foundDemon.SetControlledByPlayer();
                    }
                }
            }


            if (extraDemonsControlled.Count > 1)
            {
                controllingMultipleDemons = true;
                if (extraDemonsControlled.Count > 1 && ControlledDemon == null)
                {
                    ControlledDemon = extraDemonsControlled[0];
                    extraDemonsControlled.Remove(ControlledDemon);
                }
                InputManager.Instance.UpdateExtraDemonsControlled(extraDemonsControlled);
                InputManager.Instance.UpdateDemonReference();
            }
            else if (extraDemonsControlled.Count == 1)
            {
                ControlledDemon = extraDemonsControlled[0];
                extraDemonsControlled.Remove(ControlledDemon);
                InputManager.Instance.UpdateExtraDemonsControlled(extraDemonsControlled);
                InputManager.Instance.UpdateDemonReference();
            }
            else if (extraDemonsControlled.Count == 0 && ControlledDemon == null)
            {
                LevelManager.Instance.StartRestartingLevel();
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
                DemonBase foundDemon = other[i].GetComponentInParent<DemonBase>();

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
        //Debug.LogError("Possessing nearest demon to " + currentDemon.name);
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon.transform);//demonShowingSkull;//

        ControlledDemon = null;
        InputManager.Instance.UpdateDemonReference();



        if (demonToPossess != null)
        {
            //Debug.LogError("Trying to possess: " + demonToPossess.name);
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
        // Debug.LogError("Single possession of demon: " + demonToPossess.name);
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
