using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Parent class of any demon that can be controlled by the player
/// </summary>
public abstract class DemonBase : MonoBehaviour
{
    //Member variables
    private bool     m_isRagdollActive;
    private bool     m_isControlledByPlayer;
    private float    groundOffset;


    //Ragdoll child references
    private Collider2D[]    m_limbsColliders;
    private Rigidbody2D[]   m_limbsRbds;
    private LayerMask       mask; 

    //Demon references
    private Rigidbody2D m_myRgb;
    private Collider2D  m_myCollider;

    #region Properties

    protected bool      IsControlledByPlayer { get => m_isControlledByPlayer; }
    protected bool      IsRagdollActive { get => m_isRagdollActive; }
    public Rigidbody2D  MyRgb { get => m_myRgb; }
    public Collider2D   MyCollider { get => m_myCollider; }

    #endregion

    private void Awake()
    {
        m_limbsColliders    = ReturnComponentsInChildren<Collider2D>();
        m_limbsRbds         = ReturnComponentsInChildren<Rigidbody2D>();
        m_myRgb             = GetComponent<Rigidbody2D>();
        m_myCollider        = GetComponent<Collider2D>();
        mask                = LayerMask.NameToLayer("Default");
        SetGroundOffset();
    }

    private void SetGroundOffset()
    {
        RaycastHit2D impact = Physics2D.Raycast(transform.position, Vector2.down, 3, mask);
        groundOffset = impact.distance;
    }

    /// <summary>
    /// Method that returns the child gameobjects with the specified component, without the parent
    /// </summary>
    /// <typeparam name="T">The specified component that the method will look for</typeparam>
    /// <returns></returns>
    private T[] ReturnComponentsInChildren<T>()
    {
        T[] array = GetComponentsInChildren<T>();
        T[] returnedArray = new T[array.Length - 1];

        for (int i = 1; i < array.Length; i++)
        {
            returnedArray[i - 1] = array[i];
        }

        return returnedArray;
    }

    /// <summary>
    /// Sets the demon to be the one controlled by the player and turns off ragdoll physics
    /// </summary>
    public void SetControlledByPlayer()
    {
        m_isControlledByPlayer = true;
        SetRagdollActive(false);

        
        Transform childObject = transform.GetChild(0);
        childObject.parent = null;

        RaycastHit2D impact = Physics2D.Raycast(childObject.position, Vector2.down, 3, mask);
        float torsoOffset = impact.distance;

        transform.position = new Vector2(childObject.transform.position.x, childObject.transform.position.y + groundOffset - torsoOffset);           

        childObject.parent = transform;
        childObject.localPosition = new Vector2(childObject.localPosition.x, -groundOffset + torsoOffset);
    }
    
    /// <summary>
    /// Uses the active skill of the demon
    /// </summary>
    public abstract void UseSkill();

    protected virtual void Update()
    {
        print(LayerMask.LayerToName(mask));
    }


    /// <summary>
    /// Toggles the ragdoll physics of the demon
    /// </summary>
    /// <param name="active">True to activate ragdoll physics, false to turn them off </param>
    private void SetRagdollActive(bool active)
    {
        m_isRagdollActive = active;
               
        //activate all the limbs colliders if ragdoll is active, set inactive otherwise
        for (int i = 0; i < m_limbsColliders.Length; i++)
        {
            m_limbsColliders[i].enabled = active;
        }
        
        //set all the limbs as dynamic if ragdoll is active, kinematic otherwise
        for (int i = 0; i < m_limbsRbds.Length; i++)
        {
            m_limbsRbds[i].isKinematic = !active;

            //reset velocity in case the player will control it
            if (!active)
            {
                m_limbsRbds[i].velocity = Vector2.zero;
                m_limbsRbds[i].angularVelocity = 0;
            }
        }

        //toggle the collider and the rigidbody of the parent gameobject
        m_myCollider.enabled = !active;
        m_myRgb.isKinematic = active;
    }

    /// <summary>
    /// Sets the demon to be no longer controlled by the player and activates ragdoll physics
    /// </summary>
    public void SetNotControlledByPlayer()
    {
        m_isControlledByPlayer = false;
        SetRagdollActive(true);
        m_myRgb.velocity = Vector2.zero;
    }

    private DemonBase LookForNearestDemon(int radiusLimit)
    {
        int lookForRadius = 1;
        LayerMask lm = LayerMask.NameToLayer("Ragdoll");
        print(LayerMask.LayerToName(lm));
        while (lookForRadius <= radiusLimit)
        {
            Collider2D other = Physics2D.OverlapCircle(transform.position, lookForRadius, lm);
            if(other != null)
            {
                print(other.transform.root.name);
                return other.transform.root.GetComponent<DemonBase>();
            }
            else
            {
                lookForRadius++;
            }            
        }
        return null;
    }

    public void PossessNearestDemon(int radiusLimit)
    {
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit);
        if(demonToPossess != null)
        {
            demonToPossess.SetControlledByPlayer();
            SetNotControlledByPlayer();
        }
        

    }

}
