using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PosesionManager : TemporalSingleton<PosesionManager>
{

    public DemonBase m_controlledDemon;    
    public DemonBase ControlledDemon { get => m_controlledDemon; set => m_controlledDemon = value; }

    private void Start()
    {
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
					if (!foundDemon.IsInDanger && foundDemon.IsDead)
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
        ControlledDemon.SetNotControlledByPlayer();
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon);
        
        if (demonToPossess != null)
        {
            demonToPossess.enabled = true;
            demonToPossess.SetControlledByPlayer();
            CameraManager.Instance.ChangeCamTarget();
            InputManager.Instance.UpdateDemonReference();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
