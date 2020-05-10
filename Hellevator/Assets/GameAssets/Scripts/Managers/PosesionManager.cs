using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PosesionManager : PersistentSingleton<PosesionManager>
{
    public DemonBase m_controlledDemon;
    public DemonBase ControlledDemon { get => m_controlledDemon; set => m_controlledDemon = value; }
    public PossessingLight PLight { get => m_pLight; set => m_pLight = value; }

    [SerializeField] GameObject m_PossessionLight;
    private PossessingLight m_pLight;

    private void Start()
    {
        InputManager.Instance.UpdateDemonReference();        
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
        ControlledDemon.SetNotControlledByPlayer();
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit, currentDemon);
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
            m_pLight.Begin(demonToPossess);

			CameraManager.Instance.ChangeFocusOfMainCameraTo(m_pLight.transform);
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
        demonToPossess.SetControlledByPlayer();

        //CameraManager.Instance.ChangeCamTarget();
        InputManager.Instance.UpdateDemonReference();
		if(m_pLight != null)
		{
			m_pLight.gameObject.SetActive(false);
		}
    }
}
