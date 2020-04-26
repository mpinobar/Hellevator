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
    private bool    m_isRagdollActive;
    private bool    m_isControlledByPlayer;
    private bool    m_isInDanger;
    private bool    m_isJumping;
    private float   m_movementDirection;
    private bool    m_isLerpingToResetBones;
    private bool    m_hasResetParentPosition;
    private bool    m_isPossessionBlocked;
    protected bool  m_isDead;
    private bool    m_grabbedByRight;

    //Weight variables
    [Header("Physicality")]
    [Tooltip("Weight of the body for puzzles")]
    [SerializeField] protected float m_weight;
    [Tooltip("Speed at which the body recovers from ragdoll to idle pose")]
    [SerializeField] private float m_recomposingSpeed = 3;
    [SerializeField] private float m_recomposingDistanceMargin = 0.05f;


    [Header("Possession")]
    [SerializeField] private bool     m_possessedOnStart;
    [SerializeField] private float    m_maximumPossessionRange;
    [SerializeField] private Color    m_tintWhenCantBePossessed;
	//IAReferences
	[Space]
	[Header("IA")]
	[Tooltip("Where or not the enemy starts controlled by IA behaviour")]
	[SerializeField] protected bool         m_isControlledByIA = false;
    [SerializeField] protected float        m_IADetectionRange = 0f;
    [SerializeField] protected float        m_IADetectionAngle = 0f;
    [SerializeField] protected float        m_IADetectionRayCount = 0f;
    [SerializeField] protected LayerMask    m_IADetectionLayers;
    [SerializeField] protected LayerMask    m_IADetectionLayersForForwardVector;

    //Ragdoll child references
    private Collider2D[]        m_limbsColliders;
    private Rigidbody2D[]       m_limbsRbds;
    private Transform[]         m_childTransforms;
    private RagdollTransform[]  m_childInitialTransforms;
    private SpriteRenderer[]    m_childSprites;

    //mask for ground detection
    [Header("Don't touch")]
    [SerializeField] protected LayerMask m_defaultMask;
    [SerializeField] protected LayerMask m_JumpMask;

    //Demon references
    private Rigidbody2D     m_myRgb;
    private Collider2D      m_myCollider;
    protected Animator      m_myAnimator;
    private SpriteRenderer  m_mySprite;
    [SerializeField] SpriteRenderer m_PossessionCircle;


	//Grab Variables
	[Space]
	[Header("Grab variables")]
	private float m_hi = 0f;
    private Vector3 m_initialPositionRightGrab;
    private Vector3 m_initialPositionLeftGrab;

    /*
    [SerializeField] private Transform m_grabRayStartPositionRight;
    [SerializeField] private Transform m_grabRayStartPositionLeft;

    private bool m_hasADemonGrabed = false;
    */
    #region Properties

    public bool         IsControlledByPlayer { get => m_isControlledByPlayer; set { m_isControlledByPlayer = value; } }
    protected bool      IsRagdollActive { get => m_isRagdollActive; }
	
	public Rigidbody2D      MyRgb { get => m_myRgb; }
    public Collider2D       MyCollider { get => m_myCollider; }
	public float            Weight { get => m_weight; }
    public Collider2D[]     LimbsColliders { get => m_limbsColliders; }
    public float            MovementDirection { get => m_movementDirection; set => m_movementDirection = value; }
    public SpriteRenderer   MySprite { get => m_mySprite; }
    public bool             IsDead { get => m_isDead; set => m_isDead = value; }
    public bool 
        IsInDanger {
        get => m_isInDanger;
        set
        {
            if (value)
            {
                SetColor(m_tintWhenCantBePossessed);
            }
            else
            {
                SetColor(Color.white);
            }
            m_isInDanger = value;
        }
    }



    public bool IsPossessionBlocked
    {
        get => m_isPossessionBlocked;
        set
        {
            if (value)
            {
                SetColor(m_tintWhenCantBePossessed);
            }
            else
            {
                SetColor(Color.white);
            }
            m_isPossessionBlocked = value;
        }
    }

    public float MaximumPossessionRange { get => m_maximumPossessionRange; set => m_maximumPossessionRange = value; }


    #endregion

    protected virtual void Awake()
    {
        m_limbsColliders            = transform.GetChild(0).GetComponentsInChildren<Collider2D>();
        m_limbsRbds                 = transform.GetChild(0).GetComponentsInChildren<Rigidbody2D>();     
        m_myRgb                     = GetComponent<Rigidbody2D>();
        m_myCollider                = transform.GetChild(1).GetComponent<Collider2D>();
        m_childInitialTransforms    = SaveRagdollInitialTransform();
        m_childTransforms           = ReturnComponentsInChildren<Transform>();
        m_myAnimator                = GetComponent<Animator>();
        m_childSprites              = ReturnComponentsInChildren<SpriteRenderer>();
        m_mySprite                  = GetComponent<SpriteRenderer>();
        /*
        m_initialPositionLeftGrab   = m_grabRayStartPositionLeft.localPosition;
        m_initialPositionRightGrab  = m_grabRayStartPositionRight.localPosition;
        */
        if (m_possessedOnStart)
        {
            SetControlledByPlayer();
        }
        else
        {
            if (m_isControlledByIA)
            {
                SetControlledByAI();
            }
            else
			{
				SetNotControlledByPlayer();
			}
            
        }        
    }

	protected virtual void Update()
	{
		if (m_isLerpingToResetBones)
		{
			LerpResetRagdollTransforms();
		}

        /*
        if (m_hasADemonGrabed)
        {
            if (!m_grabRayStartPositionRight.GetComponent<SpringJoint2D>().connectedBody.GetComponentInParent<DemonBase>().IsTorsoGrounded())
            {
                m_grabRayStartPositionRight.GetComponent<SpringJoint2D>().frequency = 0;
            }

            if (m_grabbedByRight)
            {
                if (!m_grabRayStartPositionRight.GetComponent<SpringJoint2D>().connectedBody.GetComponentInParent<DemonBase>().IsTorsoGrounded())
                {
                    m_grabRayStartPositionRight.GetComponent<SpringJoint2D>().frequency = 0;
                }
                else
                {
                    m_grabRayStartPositionRight.GetComponent<SpringJoint2D>().frequency = 100;
                }

                if (MovementDirection == 1)
                {
                    //want to push to the right
                    m_grabRayStartPositionRight.localPosition = m_initialPositionRightGrab + Vector3.right * 4f;
                }
                else
                {
                    //dragging towards the left
                    m_grabRayStartPositionRight.localPosition = m_initialPositionRightGrab;
                }
            }
            else
            {
                if (!m_grabRayStartPositionLeft.GetComponent<SpringJoint2D>().connectedBody.GetComponentInParent<DemonBase>().IsTorsoGrounded())
                {
                    m_grabRayStartPositionLeft.GetComponent<SpringJoint2D>().frequency = 0;
                }
                else
                {
                    m_grabRayStartPositionLeft.GetComponent<SpringJoint2D>().frequency = 100;
                }


                if (MovementDirection == 1)
                {
                    //dragging towards the right
                    m_grabRayStartPositionLeft.localPosition= m_initialPositionLeftGrab;
                }
                else
                {
                    //want to push to the left
                    m_grabRayStartPositionLeft.localPosition = m_initialPositionLeftGrab - Vector3.right * 4f;
                }
            }
        }
        */
	}

	#region Grab

    /*
	/// <summary>
	/// The demon tries to grab a dead demon
	/// </summary>
	public void Grab()
	{
		bool isLookingRight = !m_mySprite.flipX;
		RaycastHit2D rayNormal;
		if (isLookingRight)
		{
			rayNormal = Physics2D.Raycast(m_grabRayStartPositionRight.position, this.transform.up, 2, m_IADetectionLayersForForwardVector);
		}
		else
		{
			rayNormal = Physics2D.Raycast(m_grabRayStartPositionLeft.position, this.transform.up, 2, m_IADetectionLayersForForwardVector);
		}

		float angleIncrease = m_IADetectionAngle / m_IADetectionRayCount;
		
		Vector2 forwardVector = Vector2.zero;

		float angle = 0f;

		if (isLookingRight)
		{
			if (GetAngleFromVector(rayNormal.normal) >= 90)
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = Vector2.Angle(forwardVector, this.transform.right);

				angle = angle - m_IADetectionAngle / 2f;
			}
			else
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = -Vector2.Angle(forwardVector, this.transform.right);

				angle = angle - m_IADetectionAngle / 2f;
			}
		}
		else
		{

			if (GetAngleFromVector(rayNormal.normal) >= 90)
			{
				forwardVector = -Vector2.Perpendicular(rayNormal.normal);
				angle = Vector2.Angle(forwardVector, this.transform.right);

				angle = 180 + angle + m_IADetectionAngle / 2f;
			}
			else
			{
				forwardVector = Vector2.Perpendicular(rayNormal.normal);
				angle = -Vector2.Angle(forwardVector, -this.transform.right);

				angle = 180 + angle + m_IADetectionAngle / 2f;
			}
		}

		for (int i = 0; i <= m_IADetectionRayCount; i++)
		{
			Vector3 rayDirection = GetVectorFromAngle(angle);

			RaycastHit2D[] hits;

			if (isLookingRight)
			{
				hits = Physics2D.RaycastAll(m_grabRayStartPositionRight.position, rayDirection, m_IADetectionRange, m_IADetectionLayers);

				if (i == 0)
				{
					Debug.DrawRay(m_grabRayStartPositionRight.position, rayDirection, Color.cyan, 3f);
				}
				else
				{
					Debug.DrawRay(m_grabRayStartPositionRight.position, rayDirection, Color.blue, 3f);
				}
			}
			else
			{
				hits = Physics2D.RaycastAll(m_grabRayStartPositionLeft.position, rayDirection, m_IADetectionRange, m_IADetectionLayers);

				if (i == 0)
				{
					Debug.DrawRay(m_grabRayStartPositionLeft.position, rayDirection, Color.cyan, 3f);
				}
				else
				{
					Debug.DrawRay(m_grabRayStartPositionLeft.position, rayDirection, Color.blue, 3f);
				}
			}

			for (int y = 0; y < hits.Length; y++)
			{
				if (hits[y].collider.GetComponentInParent<DemonBase>() != null)
				{
					if(hits[y].collider.transform.root != this.transform)
					{
						GameObject parent = hits[y].collider.GetComponentInParent<DemonBase>().gameObject;
						if (parent.transform.GetChild(0).gameObject == hits[y].collider.gameObject)
						{
							//hits[y].collider.GetComponent<SpringJoint2D>().connectedBody = this.m_myRgb;

							m_hasADemonGrabed = true;

							//hits[y].collider.GetComponent<GrabbedTorso>().IsGrabbed = true;
							if (isLookingRight)
							{
								hits[y].collider.GetComponent<SpringJoint2D>().connectedBody = m_grabRayStartPositionRight.GetComponent<Rigidbody2D>();
                                m_grabbedByRight = true;
							}
							else
							{
								hits[y].collider.GetComponent<SpringJoint2D>().connectedBody = m_grabRayStartPositionLeft.GetComponent<Rigidbody2D>();
                                m_grabbedByRight = false;
							}



							//hits[y].collider.transform.parent.transform.SetParent(this.transform);
							//hits[y].collider.transform.SetParent(this.transform);

							//hits[y].collider.GetComponent<DistanceJoint2D>().connectedBody = this.GetComponent<Rigidbody2D>();

							//hits[y].collider.GetComponent<Rigidbody2D>().isKinematic = true;

							break;
						}
					}
				}
			}

			

			if (isLookingRight)
			{
				angle = angle + angleIncrease;
			}
			else
			{
				angle = angle - angleIncrease;
			}
		}
	}
    */
	#endregion Grab

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
	/// Sets the demon to be controlled by the AI and turns off ragdoll physics
	/// </summary>
	public void SetControlledByAI()
	{
		SetRagdollActive(false);
		m_isControlledByIA = true;
		//m_isLerpingToResetBones = true;
		m_hasResetParentPosition = false;
        m_isControlledByPlayer = false;
        m_isDead = false;
    }

    /// <summary>
    /// Sets the demon to be the one controlled by the player and turns off ragdoll physics
    /// </summary>
    public void SetControlledByPlayer()
    {
        m_isDead = false;
        SetRagdollActive(false);
        PosesionManager.Instance.ControlledDemon = this;
        m_isLerpingToResetBones = true;
        m_hasResetParentPosition = false;
		m_isControlledByIA = false;
        IsControlledByPlayer = true;
        m_PossessionCircle.enabled = true;
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
	/// Revert gravity to normal status during a Jump
	/// </summary>
	public abstract void JumpReleaseButton();

    /// <summary>
    /// Activates or deactivates the walking particles
    /// </summary>
    /// <param name="active">True to turn them on, false to turn them off</param>
    public abstract void ToggleWalkingParticles(bool active);


    /// <summary>
    /// Changes the color of all limbs
    /// </summary>
    /// <param name="color">The new color to be assigned</param>
    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
        for (int i = 0; i < m_limbsRbds.Length; i++)
        {
            m_limbsRbds[i].GetComponent<SpriteRenderer>().material.color = color;
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

		//m_isControlledByIA = false;	
	}

    /// <summary>
    /// Sets the demon to be no longer controlled by the player and activates ragdoll physics
    /// </summary>
    public void SetNotControlledByPlayer()
    {
        m_mySprite.enabled = false;
        IsControlledByPlayer = false;
        m_isDead = true;
        SetRagdollActive(true);
        this.enabled = false;
        for (int i = 0; i < m_childSprites.Length; i++)
        {
            if(m_childSprites[i] != m_PossessionCircle)
            {
                m_childSprites[i].enabled = true;
            }
            
        }
        m_PossessionCircle.enabled = false;
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
            //Debug.DrawRay(torso.position, Vector2.down*Mathf.Infinity, Color.red, 3);
            RaycastHit2D impact = Physics2D.Raycast(torso.position, Vector2.down, Mathf.Infinity, m_defaultMask);
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
            
            m_hasResetParentPosition = false;
            m_mySprite.enabled = true;
            for (int i = 0; i < m_childSprites.Length; i++)
            {
                if (m_childSprites[i] != m_PossessionCircle)
                    m_childSprites[i].enabled = false;
            }
        }
    }

    /// <summary>
    /// Die method for characters
    /// </summary>
    public virtual void Die()
    {
        MyRgb.velocity = Vector2.zero;
        ToggleWalkingParticles(false);
        m_isDead = true;
        if (MovementDirection == -1)
        {
            m_childTransforms[0].localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            m_childTransforms[0].localScale = Vector3.one;
        }
        if (m_isControlledByPlayer)
        {
            PosesionManager.Instance.PossessNearestDemon(m_maximumPossessionRange, this);
        }
        else if (m_isControlledByIA)
        {
            m_isControlledByIA = false;
            SetRagdollActive(true);
        }
    }
    /// <summary>
    /// Check to see if the character is grounded
    /// </summary>
    /// <returns>Boolean determining if it is touching the ground</returns>
    public bool IsGrounded()
    {
        Debug.DrawRay(transform.position, Vector3.down * 1.5f, Color.red);
        RaycastHit2D[] impact = Physics2D.CircleCastAll(transform.position, 0.1f, Vector2.down, 1.5f, m_JumpMask);
        bool isGrounded = false;
        for (int i = 0; i < impact.Length; i++)
        {
            if (impact[i].transform.root != transform)
            {
                isGrounded = true;
            }
        }
        return isGrounded;
    }


    //public bool IsTorsoGrounded()
    //{
    //    Debug.DrawRay(transform.position, Vector3.down * 2, Color.red);
    //    RaycastHit2D[] impact = Physics2D.CircleCastAll(transform.GetChild(0).position, 0.1f, Vector2.down, 3, m_JumpMask);
    //    bool isGrounded = false;
    //    for (int i = 0; i < impact.Length; i++)
    //    {
    //        if (impact[i].transform.root != transform)
    //        {
    //            isGrounded = true;
    //        }
    //    }
    //    return isGrounded;
    //}




	#region AngleCalculations
	protected Vector3 GetVectorFromAngle(float angle)
	{
		float angleRad = angle * (Mathf.PI / 180f);
		return new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad));
	}

	protected float GetAngleFromVector(Vector3 dir)
	{

		dir = dir.normalized;
		float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		if (n < 0) n += 360;

		return n;
	}
	#endregion AngleCalculations
}
