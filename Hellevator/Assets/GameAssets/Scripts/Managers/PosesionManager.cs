using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PosesionManager : TemporalSingleton<PosesionManager>
{
    private bool m_usingTimeStop;
    private bool m_pickingDemon;
    private DemonBase m_controlledDemon;
    public DemonBase ControlledDemon { get => m_controlledDemon; set => m_controlledDemon = value; }

    private void Start()
    {
        m_usingTimeStop = false;
        Cursor.visible = false;
        InputManager.Instance.UpdateDemonReference();
        CameraManager.Instance.ChangeCamTarget();
    }
    /// <summary>
    /// Returns the nearest demon to the demon currently controlled by the player, with a distance limit
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Transform of the currently possessed demon</param>
    /// <returns></returns>
    private DemonBase LookForNearestDemon(float radiusLimit, DemonBase currentDemon)
    {
        int lookForRadius = 1;

        while (lookForRadius <= radiusLimit)
        {
            Collider2D[] other = Physics2D.OverlapCircleAll(currentDemon.transform.position, lookForRadius);
            for (int i = 0; i < other.Length; i++)
            {
                DemonBase foundDemon = other[i].transform.root.GetComponent<DemonBase>();

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

    /// <summary>
    /// Switches the player controls to a new demon
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Currently possessed demon</param>
    public void PossessNearestDemon(float radiusLimit, DemonBase currentDemon)
    {
        if (m_usingTimeStop)
        {
            PossessTimeStop();
        }
        else
        {
            ControlledDemon.SetNotControlledByPlayer();
            DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon);
            SetDemonControlled(demonToPossess);
        }
    }


    private void Update()
    {
        if (m_pickingDemon)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Collider2D col = Physics2D.OverlapCircle(Camera.main.ScreenToWorldPoint(Input.mousePosition), 0.25f);
                if (col != null)
                {
                    DemonBase demon = col.transform.root.GetComponent<DemonBase>();
                    if (demon != null)
                    {
                        ControlledDemon.SetNotControlledByPlayer();
                        SetDemonControlled(demon);
                        Cursor.visible = false;
                        m_pickingDemon = false;
                    }
                }

            }
        }
    }

    private static void SetDemonControlled(DemonBase demonToPossess)
    {
        Time.timeScale = 1;
        if (demonToPossess != null)
        {
            demonToPossess.enabled = true;
            demonToPossess.SetControlledByPlayer();
            CameraManager.Instance.ChangeCamTarget();
            InputManager.Instance.UpdateDemonReference();
        }
        else
        {
            LevelManager.Instance.RestartLevel();
        }
    }

    public void PossessTimeStop()
    {
        Time.timeScale = 0f;
        m_pickingDemon = true;
        Cursor.visible = true;
    }

}
