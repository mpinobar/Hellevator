﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosesionManager : TemporalSingleton<PosesionManager>
{

    private DemonBase m_controlledDemon;    
    public DemonBase ControlledDemon { get => m_controlledDemon; set => m_controlledDemon = value; }

    public override void Awake()
    {
        base.Awake();
    }
    /// <summary>
    /// Returns the nearest demon to the demon currently controlled by the player, with a distance limit
    /// </summary>
    /// <param name="radiusLimit">Maximum radius to look for a demon to possess</param>
    /// <param name="currentDemon">Transform of the currently possessed demon</param>
    /// <returns></returns>
    private DemonBase LookForNearestDemon(int radiusLimit, DemonBase currentDemon)
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
					if (!foundDemon.IsInDanger)
					{
						//print("FOUND: " + foundDemon.name);
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
    public void PossessNearestDemon(int radiusLimit, DemonBase currentDemon)
    {
        ControlledDemon.SetNotControlledByPlayer();
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon);
        
        if (demonToPossess != null)
        {
            demonToPossess.enabled = true;
            demonToPossess.SetControlledByPlayer();            
        }
    }
}
