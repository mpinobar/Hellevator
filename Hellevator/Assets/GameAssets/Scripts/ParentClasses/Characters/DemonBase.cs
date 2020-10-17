using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.U2D.IK;

[SelectionBase]
/// <summary>
/// Parent class of any demon that can be controlled by the player
/// </summary>
public abstract class DemonBase : MonoBehaviour
{
    #region Variables

    //Member variables
    private bool        m_isRagdollActive;
    private bool        m_isInDanger;
    protected bool      m_isJumping;
    private float       m_movementDirection;
    private bool        m_isLerpingToResetBones;
    private bool        m_hasResetParentPosition;
    protected bool      m_isDead;
    private bool        m_grabbedByRight;
    private Color       m_outlineColorWhenControlledByPlayer;
    private float       m_initialGlowThickness;
    private bool        m_canMove;
    private float       m_distanceToPlayer;

    [Space]
    //Weight variables
    [Header("Physicality")]
    
    [Tooltip("Weight of the body for puzzles")]
    [SerializeField] protected float        m_weight;

    [Tooltip("Speed at which the body recovers from ragdoll to idle pose")]
    [SerializeField] private float          m_recomposingSpeed = 3;
    [SerializeField] private float          m_recomposingDistanceMargin = 0.05f;
    [SerializeField] private Collider2D     m_ragdollLogicCollider;
    [SerializeField] protected Transform    m_torso;
    private float                           m_dragMovement = 0f;

    [Space]
    [Header("Audio")]
    [SerializeField] protected AudioClip m_deathClip;
    [SerializeField] protected AudioClip m_jumpClip;
    [SerializeField] protected AudioClip m_landingClip;

    [Space]
    [Header("Possession")]
    [SerializeField] private bool       m_possessedOnStart;
    [SerializeField] private bool       m_multiplePossessionWhenDead;
    [SerializeField] private bool       m_startsStandingUp;
    [SerializeField] private float      m_maximumPossessionRange;

    [ColorUsage(true, true)]
    [SerializeField] private Color      m_tintWhenCantBePossessed;
    [SerializeField] SpriteRenderer     m_PossessionCircle;
    private float                       m_distanceStartGlow = 10;

    [ColorUsage(true, true)]
    [SerializeField] private Color      m_colorWhenAvailable;

    private GameObject                  m_spiritFire;
    private float                       m_distanceMaxGlow = 5;
    private bool                        m_isPossessionBlocked;
    private bool                        m_isControlledByPlayer;

    [ColorUsage(true,true)]
    [SerializeField] private Color      m_fireColorWhenPossessed;
    [SerializeField] private Color      m_spritesColor;
    [ColorUsage(true, true)] Color      m_fireColorWhenNotPossessed;
    [SerializeField] GameObject         overlay;

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
    private IKManager2D         m_IKManager;
    [Space]
    //mask for ground detection
    [Header("Don't touch")]
    [SerializeField] protected LayerMask    m_groundedDetectionLayers;
    [SerializeField] protected GameObject   m_demonMaskSprite;

    //Demon references
    private Rigidbody2D     m_myRgb;
    private Collider2D      m_playerCollider;
    protected Animator      m_myAnimator;



    /*
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
    #endregion

    #region Properties

    public bool IsControlledByPlayer
    {
        get => m_isControlledByPlayer; set
        {
            m_isControlledByPlayer = value;
        }
    }
    protected bool IsRagdollActive
    {
        get => m_isRagdollActive;
    }

    public Rigidbody2D MyRgb
    {
        get => m_myRgb;
    }
    public Collider2D PlayerCollider
    {
        get => m_playerCollider;
    }
    public float Weight
    {
        get => m_weight;
    }
    public Collider2D[] LimbsColliders
    {
        get => m_limbsColliders;
    }
    public float MovementDirection
    {
        get => m_movementDirection; set => m_movementDirection = value;
    }
    public bool IsDead
    {
        get => m_isDead; set => m_isDead = value;
    }
    public bool
        IsInDanger
    {
        get => m_isInDanger;
        set
        {
            if (value)
            {
                //SetColor(m_tintWhenCantBePossessed);
            }
            else
            {
                //SetColor(m_spritesColor);
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
                //SetColor(m_tintWhenCantBePossessed);
            }
            else
            {
                //SetColor(m_spritesColor);
            }
            m_isPossessionBlocked = value;
        }
    }

    public float MaximumPossessionRange
    {
        get => m_maximumPossessionRange; set => m_maximumPossessionRange = value;
    }
    public Collider2D RagdollLogicCollider
    {
        get => m_ragdollLogicCollider; set => m_ragdollLogicCollider = value;
    }
    public Transform Torso
    {
        get => m_torso; set => m_torso = value;
    }
    public bool m_hasTurnedOff
    {
        get; private set;
    }
    public Transform[] ChildTransforms
    {
        get => m_childTransforms; set => m_childTransforms = value;
    }
    public bool CanMove
    {
        get => m_canMove; set => m_canMove = value;
    }
    public float DistanceToPlayer { get => m_distanceToPlayer; set => m_distanceToPlayer = value; }
    public bool MultiplePossessionWhenDead { get => m_multiplePossessionWhenDead; set => m_multiplePossessionWhenDead = value; }


    #endregion

    protected virtual void Awake()
    {
        m_limbsColliders = m_torso.GetComponentsInChildren<Collider2D>();
        m_limbsRbds = GetComponentsInChildren<Rigidbody2D>();
        m_myRgb = GetComponent<Rigidbody2D>();
        m_playerCollider = GetComponent<Collider2D>();
        m_childInitialTransforms = SaveRagdollInitialTransform();
        m_childTransforms = m_torso.GetComponentsInChildren<Transform>();
        m_myAnimator = GetComponent<Animator>();
        //m_childSprites = GetComponentsInChildren<SpriteRenderer>(true);
        //m_outlineColorWhenControlledByPlayer = m_childSprites[3].material.GetColor("Color_A7D64A79");
        //m_initialGlowThickness = m_childSprites[3].material.GetFloat("_Thickness");
        //m_IKManager = GetComponent<IKManager2D>();

        if (overlay)
        {
            overlay.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, 0);

            overlay.SetActive(false);
        }else {
            Debug.LogError("No se ha asignado la referencia al overlay para mostrar rango de posesión. Nombre de cuerpo: " + name);
        }


        if (m_demonMaskSprite)
            m_demonMaskSprite.SetActive(true);
        //for (int i = 0; i < m_childTransforms.Length; i++)
        //{
        //    if (m_childTransforms[i].CompareTag("FireSpirit"))
        //    {
        //        m_spiritFire = m_childTransforms[i].gameObject;
        //    }
        //}
        m_spiritFire = m_torso.GetChild(m_torso.childCount - 1).gameObject;
        m_spiritFire.SetActive(true);
        IsInDanger = false;
        m_fireColorWhenNotPossessed = m_spiritFire.GetComponent<SpriteRenderer>().material.GetColor("Color_7F039FD4");
        /*
        m_initialPositionLeftGrab   = m_grabRayStartPositionLeft.localPosition;
        m_initialPositionRightGrab  = m_grabRayStartPositionRight.localPosition;
        */

    }

    private void Start()
    {
        if (m_possessedOnStart && PossessionManager.Instance.ControlledDemon == null)
        {
            PossessionManager.Instance.ControlledDemon = this;
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
            //ResetRagdollTransforms();
            LerpResetRagdollTransforms();
        }

        if (IsControlledByPlayer)
        {

            if (Input.GetKeyDown(KeyCode.Q))
            {
                HidePossessionRange();
            }
        }
        m_spiritFire.transform.rotation = Quaternion.identity;

        #region Shader outline

        //if (!IsControlledByPlayer)
        //{
        //    if (PossessionManager.Instance.ControlledDemon != null)
        //    {

        //        m_distanceStartGlow = PossessionManager.Instance.ControlledDemon.MaximumPossessionRange;
        //        float distanceToPlayer = Vector2.Distance(Torso.position, PossessionManager.Instance.ControlledDemon.transform.position);
        //        if (distanceToPlayer < m_distanceStartGlow)
        //        {
        //            //m_hasTurnedOff = false;

        //            //START ON 1 BECAUSE NUMBER 0 IS FIRE SPRITE
        //            for (int i = 1; i < m_childSprites.Length; i++)
        //            {
        //                if (!m_isInDanger && !m_isPossessionBlocked)
        //                {
        //                    m_childSprites[i].material.SetColor("Color_A7D64A79", m_colorWhenAvailable);
        //                }
        //                else if (m_isInDanger || m_isPossessionBlocked)
        //                {
        //                    m_childSprites[i].material.SetColor("Color_A7D64A79", m_tintWhenCantBePossessed);
        //                }

        //            }

        //            for (int i = 1; i < m_childSprites.Length; i++)
        //            {
        //                m_childSprites[i].material.SetFloat("_Thickness", m_initialGlowThickness);
        //            }
        //        }
        //        else //if (!m_hasTurnedOff)
        //        {
        //            for (int i = 1; i < m_childSprites.Length; i++)
        //            {
        //                m_childSprites[i].material.SetFloat("_Thickness", 0);
        //                m_hasTurnedOff = true;

        //            }
        //        }
        //    }

        //}
        //else if (IsControlledByPlayer)
        //{

        //    if (Input.GetKeyDown(KeyCode.Q))
        //    {
        //        HidePossessionRange();
        //    }

        //    if (!m_isInDanger && !m_isPossessionBlocked)
        //    {
        //        for (int i = 1; i < m_childSprites.Length; i++)
        //        {
        //            m_childSprites[i].material.SetFloat("_Thickness", 0);
        //            m_hasTurnedOff = true;

        //        }
        //    }
        //    else if (m_isInDanger || m_isPossessionBlocked)
        //    {
        //        for (int i = 1; i < m_childSprites.Length; i++)
        //        {
        //            m_childSprites[i].material.SetFloat("_Thickness", m_initialGlowThickness);
        //            m_childSprites[i].material.SetColor("Color_A7D64A79", m_tintWhenCantBePossessed);
        //        }
        //    }

        //}

        #endregion
    }

    #region Grab
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

    INCLUDE UP TO HERE IN UPDATE
    */


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

    #region ControlAssignment

    /// <summary>
    /// Sets the demon to be no longer controlled by the player and activates ragdoll physics
    /// </summary>
    public void SetNotControlledByPlayer()
    {
        if (m_IKManager != null)
        {
            m_IKManager.enabled = false;
        }
        IsControlledByPlayer = false;
        m_isDead = true;

        if (m_startsStandingUp)
        {
            
            m_startsStandingUp = false;
            m_ragdollLogicCollider.enabled = true;
            m_myRgb.isKinematic = true;
            for (int i = 0; i < m_limbsColliders.Length; i++)
            {
                m_limbsColliders[i].enabled = true;
            }
            for (int i = 0; i < m_limbsRbds.Length; i++)
            {
                m_limbsRbds[i].isKinematic = true;
            }
        }
        else
        {
            SetRagdollActive(true);
        }
        CanMove = false;
        
        //for (int i = 1; i < m_childSprites.Length; i++)
        //{
        //    m_childSprites[i].material.SetFloat("_Thickness", 0);
        //    m_childSprites[i].sortingLayerName = "Default";
        //}
        m_spiritFire.GetComponent<SpriteRenderer>().material.SetColor("Color_7F039FD4", m_fireColorWhenNotPossessed);
        //m_spiritFire.SetActive(true);
        //m_PossessionCircle.enabled = false;
        m_myAnimator.enabled = false;
        //this.enabled = false;
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
        m_spiritFire.GetComponent<SpriteRenderer>().material.SetColor("Color_7F039FD4", m_fireColorWhenNotPossessed);
    }

    /// <summary>
    /// Sets the demon to be the one controlled by the player and turns off ragdoll physics
    /// </summary>
    public void SetControlledByPlayer()
    {
        if (m_IKManager != null)
        {
            m_IKManager.enabled = true;
        }
        m_isDead = false;
        SetRagdollActive(false);
        CanMove = true;
        m_isLerpingToResetBones = true;
        m_playerCollider.enabled = false;
        m_hasResetParentPosition = false;
        m_isControlledByIA = false;
        IsControlledByPlayer = true;
        m_spiritFire.GetComponent<SpriteRenderer>().material.SetColor("Color_7F039FD4", m_fireColorWhenPossessed);
        
        if (PossessionManager.Instance.ControlledDemon) 
        CameraManager.Instance.ChangeFocusOfMainCameraTo(PossessionManager.Instance.ControlledDemon.transform);

        if (CameraManager.Instance.CurrentCamera == CameraManager.Instance.PlayerCamera)
        {
        }
        
        //m_PossessionCircle.enabled = true;

        //for (int i = 1; i < m_childSprites.Length; i++)
        //{
        //    m_childSprites[i].material.SetFloat("_Thickness", m_initialGlowThickness);
        //    m_childSprites[i].sortingLayerName = "Player";
        //    m_childSprites[i].material.SetColor("Color_A7D64A79", m_outlineColorWhenControlledByPlayer);
        //}

    }

    #endregion

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
        for (int i = 1; i < m_childSprites.Length; i++)
        {
            m_childSprites[i].color = color;
        }
    }

    public void ShowPossessionRange()
    {
        StopAllCoroutines();
        StartCoroutine(LerpPossessionOverlay(true));
    }

    IEnumerator LerpPossessionOverlay(bool active)
    {
        overlay.transform.localScale = Vector3.one * MaximumPossessionRange * 5;
        SpriteRenderer spr = overlay.GetComponent<SpriteRenderer>();
        Color aux = spr.color;
        Color endColor = Color.black;

        overlay.SetActive(true);
        if (active)
        {
            endColor.a = 1;
        }
        else
        {
            endColor.a = 0;
        }
        while (Mathf.Abs(aux.a - endColor.a) > 0.05f)
        {
            aux = Color.Lerp(aux, endColor, 2 * Time.unscaledDeltaTime);
            spr.color = aux;
            yield return null;
        }

        spr.color = endColor;
        if (!active)
        {
            overlay.SetActive(false);
        }
    }

    public void HidePossessionRange()
    {
        StopAllCoroutines();
        StartCoroutine(LerpPossessionOverlay(false));
    }

    public void DragMovement(float amount)
    {
        if (m_dragMovement == 0 && amount != 0)
        {

            m_dragMovement = amount;
        }
        if (amount == 0)
        {
            m_dragMovement = 0;
        }
        transform.position = Vector2.MoveTowards(transform.position, transform.position + Vector3.right, m_dragMovement * Time.deltaTime);
    }

    #region Ragdolls

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

        m_playerCollider.enabled = !active;
        m_ragdollLogicCollider.enabled = active;
        m_myRgb.isKinematic = active;
        if (!active)
        {
            m_myRgb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            m_myRgb.constraints = RigidbodyConstraints2D.None;
        }

    }

    /// <summary>
    /// Saves the position and rotation of each ragdoll part with an identifier by hashed name
    /// </summary>
    /// <returns>Returns an array of RagdollTransform with the position and rotation of all the child objects</returns>
    private RagdollTransform[] SaveRagdollInitialTransform()
    {
        Transform[] aux = m_torso.GetComponentsInChildren<Transform>();
        RagdollTransform[] rdolls = new RagdollTransform[aux.Length];
        for (int i = 0; i < rdolls.Length; i++)
        {
            if (!aux[i].CompareTag("Mask"))
                rdolls[i] = new RagdollTransform(aux[i].name.GetHashCode(), aux[i].localPosition, aux[i].localRotation);
        }
        return rdolls;
    }

    /// <summary>
    /// Resets the position and rotation of all ragdoll parts immediately
    /// </summary>
    private void ResetRagdollTransforms()
    {
        //m_childTransforms = ReturnComponentsInChildren<Transform>();
        //transform.rotation = Quaternion.identity;
        for (int i = 0; i < m_childTransforms.Length; i++)
        {
            int partId = m_childTransforms[i].name.GetHashCode();

            for (int j = 0; j < m_childInitialTransforms.Length; j++)
            {
                if (!m_childTransforms[j].CompareTag("Mask") && partId == m_childInitialTransforms[j].Id)
                {
                    m_childTransforms[i].localPosition = m_childInitialTransforms[j].Position;
                    m_childTransforms[i].localRotation = m_childInitialTransforms[j].Rotation;
                }
            }
        }
    }


    /// <summary>
    /// Lerps all bones to their initial positions
    /// </summary>
    private void LerpResetRagdollTransforms()
    {
        if (!m_hasResetParentPosition)
        {
            m_torso.parent = null;
            //Debug.DrawRay(torso.position, Vector2.down*Mathf.Infinity, Color.red, 3);
            RaycastHit2D impact = Physics2D.Raycast(m_torso.position, Vector2.down, 3f, m_groundedDetectionLayers);
            if (impact.transform != null)
            {
                transform.position = impact.point;
            }
            else
            {
                //HARDCODED NUMBER FOR WHEN POSSESSING A CORPSE WHILE SAID CORPSE IS IN THE AIR
                transform.position = m_torso.transform.position - Vector3.up * 2f;
            }
            m_hasResetParentPosition = true;
            m_torso.parent = transform;
            m_playerCollider.enabled = true;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.identity, m_recomposingSpeed * Time.deltaTime);
        for (int i = 0; i < m_childInitialTransforms.Length; i++)
        {
            int partId = m_childTransforms[i].name.GetHashCode();

            for (int j = 0; j < m_childInitialTransforms.Length; j++)
            {
                if (!m_childTransforms[j].CompareTag("Mask") && partId == m_childInitialTransforms[j].Id)
                {
                    m_childTransforms[i].localPosition = Vector3.Lerp(m_childTransforms[i].localPosition, m_childInitialTransforms[j].Position, m_recomposingSpeed * Time.deltaTime);
                    m_childTransforms[i].localRotation = Quaternion.Lerp(m_childTransforms[i].localRotation, m_childInitialTransforms[j].Rotation, m_recomposingSpeed * Time.deltaTime);

                }
            }
        }
        if (Vector3.Distance(m_torso.localPosition, m_childInitialTransforms[0].Position) < m_recomposingDistanceMargin)
        {
            ResetRagdollTransforms();
            m_isLerpingToResetBones = false;
            m_myAnimator.enabled = true;
            m_hasResetParentPosition = false;
        }
    }

    #endregion

    /// <summary>
    /// Die method for characters
    /// </summary>
    public virtual void Die(bool playDeathSound)
    {
        
        MyRgb.velocity = Vector2.zero;
        ToggleWalkingParticles(false);
        if (!m_isDead && playDeathSound)
        {
            MusicManager.Instance.PlayAudioSFX(m_deathClip, false);
            m_isDead = true;
        }

        if (m_isControlledByPlayer)
        {
            //Debug.LogError("Player died: " + name);
            PossessionManager.Instance.RemoveDemonPossession(transform);
            m_isDead = true;
            UseSkill();
        }
        else if (m_isControlledByIA)
        {
            m_isControlledByIA = false;
            m_isDead = true;
            SetRagdollActive(true);
        }
    }
    /// <summary>
    /// Check to see if the character is grounded
    /// </summary>
    /// <returns>Boolean determining if it is touching the ground</returns>
    public bool IsGrounded()
    {        
        RaycastHit2D[] impact = Physics2D.CircleCastAll(transform.position, 0.3f, Vector2.down, 0.5f, m_groundedDetectionLayers);
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

    public virtual void StopMovement()
    {

    }

    public virtual void ContinueMovement()
    {

    }

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

        if (n < 0)
            n += 360;

        return n;
    }
    #endregion AngleCalculations
}
