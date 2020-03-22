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
    private bool     m_isInDanger;

  	//Weight variables
   	[SerializeField] private float m_weight;
  
    private bool     m_isLerpingToResetBones;
    private bool     m_hasResetParentPosition;


    [SerializeField] private float    m_recomposingSpeed = 3;
    [SerializeField] private float    m_recomposingDistanceMargin = 0.05f;
    [SerializeField] private bool     m_possessedOnStart;

    //Ragdoll child references
    private Collider2D[]        m_limbsColliders;
    private Rigidbody2D[]       m_limbsRbds;
    private Transform[]         m_childTransforms;
    private RagdollTransform[]  m_childInitialTransforms;

    //mask for ground detection
    [SerializeField] protected LayerMask m_defaultMask; 

    //Demon references
    private Rigidbody2D m_myRgb;
    private Collider2D  m_myCollider;

    #region Properties

    public bool      IsControlledByPlayer { get => m_isControlledByPlayer; set { m_isControlledByPlayer = value; } }
    protected bool      IsRagdollActive { get => m_isRagdollActive; }
	public bool IsInDanger { get => m_isInDanger; set => m_isInDanger = value; }
	public Rigidbody2D  MyRgb { get => m_myRgb; }
    public Collider2D   MyCollider { get => m_myCollider; }
	public float Weight { get => m_weight; set => m_weight = value; }


	#endregion

	private void Awake()
    {
        m_limbsColliders            = transform.GetChild(0).GetComponentsInChildren<Collider2D>();
        m_limbsRbds                 = transform.GetChild(0).GetComponentsInChildren<Rigidbody2D>();     
        m_myRgb                     = GetComponent<Rigidbody2D>();
        m_myCollider                = transform.GetChild(1).GetComponent<Collider2D>();
        m_childInitialTransforms    = SaveRagdollInitialTransform();
        m_childTransforms           = ReturnComponentsInChildren<Transform>();

        if (m_possessedOnStart)
        {
            SetControlledByPlayer();
        }
        else
        {
            SetNotControlledByPlayer();
        }
        
    }

    private void Start()
    {
        
    }

    /// <summary>
    /// Returns all child component references of specified component, excluding the parent
    /// </summary>
    /// <typeparam name="T">The specified component to look for</typeparam>
    /// <returns>An array with the components</returns>
    private T[] ReturnComponentsInChildren<T>()
    {
        T[] array = GetComponentsInChildren<T>();
        T[] returnedArray = new T[array.Length - 1];

        for (int i = 0; i < returnedArray.Length; i++)
        {
            returnedArray[i] = array[i + 1];
        }
        return returnedArray;
    }
    



    /// <summary>
    /// Sets the demon to be the one controlled by the player and turns off ragdoll physics
    /// </summary>
    public void SetControlledByPlayer()
    {
        SetRagdollActive(false);
        PosesionManager.Instance.ControlledDemon = this;
        m_isLerpingToResetBones = true;
        m_hasResetParentPosition = false;
    }
    
    /// <summary>
    /// Saves the position and rotation of each ragdoll part with an identifier by hashed name
    /// </summary>
    /// <returns>Returns an array of RagdollTransform with the position and rotation of all the child objects</returns>
    private RagdollTransform[] SaveRagdollInitialTransform()
    {
        Transform[] aux = ReturnComponentsInChildren<Transform>();
        RagdollTransform[] rdolls = new RagdollTransform[aux.Length];
        for (int i = 0; i < rdolls.Length; i++)
        {
            rdolls[i] = new RagdollTransform(aux[i].name.GetHashCode(), aux[i].localPosition, aux[i].localRotation);
        }
        return rdolls;
    }

    /// <summary>
    /// Uses the active skill of the demon
    /// </summary>
    public abstract void UseSkill();

    /// <summary>
    /// The standard horizontal movement
    /// </summary>
    /// <param name="xInput">Input to feed to the method, left or right</param>
    public abstract void Move(float xInput);

    /// <summary>
    /// Demon jumping
    /// </summary>
    public abstract void Jump();

    /// <summary>
    /// Activates or deactivates the walking particles
    /// </summary>
    /// <param name="active">True to turn them on, false to turn them off</param>
    public abstract void ToggleWalkingParticles(bool active);

    protected virtual void Update()
    {
        if (m_isLerpingToResetBones)
        {
            LerpResetRagdollTransforms();
        }
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
        IsControlledByPlayer = false;
        SetRagdollActive(true);
        this.enabled = false;
    }



    /// <summary>
    /// Resets the position and rotation of all ragdoll parts immediately
    /// </summary>
    private void ResetRagdollTransforms()
    {
        m_childTransforms = ReturnComponentsInChildren<Transform>();
        transform.rotation = Quaternion.identity;
        for (int i = 0; i < m_childTransforms.Length; i++)
        {
            int partId = m_childTransforms[i].name.GetHashCode();

            for (int j = 0; j < m_childInitialTransforms.Length; j++)
            {
                if(partId == m_childInitialTransforms[j].Id)
                {
                    m_childTransforms[i].localPosition = m_childInitialTransforms[j].Position;
                    m_childTransforms[i].localRotation = m_childInitialTransforms[j].Rotation;
                }
            }
        }
    }


    /// <summary>
    /// Resets the position and rotation of all ragdoll parts lerping
    /// </summary>
    private void LerpResetRagdollTransforms()
    {
        Transform torso = m_limbsColliders[0].transform;
        if (!m_hasResetParentPosition)
        {
            torso.parent = null;
            Debug.DrawRay(torso.position, Vector2.down, Color.red, 3);
            RaycastHit2D impact = Physics2D.Raycast(torso.position, Vector2.down, 3, m_defaultMask);
            transform.position = impact.point + Vector2.up * 2;
            m_hasResetParentPosition = true;
            torso.parent = transform;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, m_recomposingSpeed * Time.deltaTime);
        for (int i = 0; i < m_childTransforms.Length; i++)
        {
            int partId = m_childTransforms[i].name.GetHashCode();

            for (int j = 0; j < m_childInitialTransforms.Length; j++)
            {
                if (partId == m_childInitialTransforms[j].Id)
                {
                    m_childTransforms[i].localPosition = Vector3.Lerp(m_childTransforms[i].localPosition, m_childInitialTransforms[j].Position, m_recomposingSpeed * Time.deltaTime);
                    m_childTransforms[i].localRotation = Quaternion.Lerp(m_childTransforms[i].localRotation, m_childInitialTransforms[j].Rotation, m_recomposingSpeed * Time.deltaTime);
                    
                }
            }
        }
        //print(Vector3.Distance(torso.localPosition, m_childInitialTransforms[0].Position));
        if(Vector3.Distance(torso.localPosition, m_childInitialTransforms[0].Position) < m_recomposingDistanceMargin)
        {
            ResetRagdollTransforms();
            m_isLerpingToResetBones = false;
            IsControlledByPlayer = true;
            m_hasResetParentPosition = false;
        }
    }

}
