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

        m_limbsColliders    = transform.GetChild(0).GetComponentsInChildren<Collider2D>();
        m_limbsRbds         = transform.GetChild(0).GetComponentsInChildren<Rigidbody2D>();

     
        m_myRgb             = GetComponent<Rigidbody2D>();
        m_myCollider        = transform.GetChild(1).GetComponent<Collider2D>();
        mask                = LayerMask.NameToLayer("Default");
        SetGroundOffset();
    }

    private void SetGroundOffset()
    {
        RaycastHit2D impact = Physics2D.Raycast(transform.position, Vector2.down, 3, mask);
        groundOffset = impact.distance;
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
        m_myCollider.gameObject.SetActive(!active);
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
        DemonBase demon = null;
        while (lookForRadius <= radiusLimit)
        {
            Collider2D[] other = Physics2D.OverlapCircleAll(transform.position, lookForRadius);
            for (int i = 0; i < other.Length; i++)
            {
                DemonBase foundDemon = other[i].transform.root.GetComponent<DemonBase>();
                if (foundDemon != null && foundDemon != this)
                {
                    demon = foundDemon;
                    return demon;
                }
            }
            
            if(demon == null)
            {
                lookForRadius++;
            }            
        }
        return demon;
    }

    public void PossessNearestDemon(int radiusLimit)
    {
        DemonBase demonToPossess = LookForNearestDemon(radiusLimit);
        print(demonToPossess.name);
        if(demonToPossess != null)
        {
            demonToPossess.SetControlledByPlayer();
            SetNotControlledByPlayer();
        }
    }
}
