using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using Random = UnityEngine.Random;

public class BasicZombie : DemonBase
{

    #region Variables	


    [Header("Movement")]
    [SerializeField] List<GameObject>   m_limbsToUnparent;
    [SerializeField] private float      m_timeToDestroyLimbsAfterUnparenting = 10f;


    [SerializeField] private float      m_maxSpeed;
    [SerializeField] private float      m_acceleration = 7;
    [SerializeField] private float      m_deceleration = 7;



    [SerializeField] private float      m_jumpForce = 10;
    [SerializeField] private float      m_jumpForceSecond = 10;
    [SerializeField] private bool       m_canJump;
    [SerializeField] private bool       m_canDoubleJump;
    [SerializeField] private float      m_coyoteTimeDuration = 0f;//Mirar si hacer cambio a frames
    [SerializeField] private float      m_groundCorrectionMultiplier = 3;
    [SerializeField] private float      m_airCorrectionMultiplier = 3;
    [SerializeField] private float      m_waitTimeResetPlatformTraversal = 0.5f;
    [SerializeField] private float      m_maxFallSpeedVertical = 25f;

    private bool m_isOnLadder = false;
    private bool m_hasJumped;
    private bool m_hasDoubleJumped;
    private bool m_coyoteTimeActive = false;
    private bool m_isHoldingJump = false;
    private bool m_tryingToGrabLadder;
    private bool m_jumpHasBeenPressOnAir = false;
    private bool m_tryingToTraversePlatform = false;

    [SerializeField] private float m_jumpHasBeenPressOnAirTimer = 0f;
    private float m_currentTimerJumpOnAir = 0f;

    private float m_currentCoyoteTimer = 0f;
    private float m_previousGravityScale = 0f;
    private float m_skullOffset;
    private Vector3 m_previousVelocity = Vector3.zero;
    LayerMask ladderLayer = 1 << 12;

    [Header("References")]
    [SerializeField] ParticleSystem m_walkingParticles;
    [SerializeField] bool m_SoyUnNiñoDeVerdad;
    [SerializeField] GameObject m_skullIndicator;
    [SerializeField] ParticleSystem m_jumpLandParticles;

    [Header("Gravity")]
    [Range(1,20)]
    [Tooltip("Ascending part of the jump")]
    [SerializeField] private float m_firstGravity = 2.25f;
    [Range(1, 20)]
    [Tooltip("Ascending part of the jump when holding the jump button")]
    [SerializeField] private float m_firstGravityHoldingJump = 2.25f;
    [Range(1, 20)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_secondGravity = 2.5f;
    [Range(1, 20)]
    [Tooltip("First top part of the jump")]
    [SerializeField] private float m_thirdGravity = 2.5f;
    [Range(1, 20)]
    [Tooltip("Second top part of the double jump")]
    [SerializeField] private float m_thirdGravityDoubleJump = 2f;
    [Range(1, 20)]
    [Tooltip("Descending part of the jump")]
    [SerializeField] private float m_fourthGravity = 5f;
    [SerializeField] private float m_ySpeedWhenReleasingEarly = 3f;

    [Header("Color")]
    //[ColorUsage(true,true)]
    //[SerializeField] Color m_colorSkullIndicatorExplosive;
    //[ColorUsage(true,true)]
    //[SerializeField] Color m_colorSkullIndicatorPetrification;
    [SerializeField] GameObject m_faceCover;

    #endregion

    #region Properties
    public float MaxSpeed { get => m_maxSpeed; set => m_maxSpeed = value; }
    public float Acceleration { get => m_acceleration; }
    public float JumpForce { get => m_jumpForce; }
    public bool SoyUnNiñoDeVerdad { get => m_SoyUnNiñoDeVerdad; set => m_SoyUnNiñoDeVerdad = value; }
    public bool TryingToGrabLadder { get => m_tryingToGrabLadder; set => m_tryingToGrabLadder = value; }
    public bool IsOnLadder { get => m_isOnLadder; }
    public bool CanJump { get => m_canJump; set => m_canJump = value; }

    #endregion


    public Action OnJumped;

    public override void UseSkill()
    {
        if (GetComponent<Petrification>())
            GetComponent<Petrification>().Petrify();
        else if (GetComponent<Explosion>())
            GetComponent<Explosion>().CreateExplosion();
    }
    protected override void Awake()
    {
        base.Awake();
        m_faceCover.SetActive(false);
        if (m_skullIndicator)
        {

            m_skullOffset = m_skullIndicator.transform.position.y - m_torso.transform.position.y;
            //if (GetComponent<Explosion>() != null)
            //{
            //    m_skullIndicator.GetComponent<SpriteRenderer>().color = m_colorSkullIndicatorExplosive;
            //}
            //else if (GetComponent<Petrification>() != null)
            //{
            //    m_skullIndicator.GetComponent<SpriteRenderer>().color = m_colorSkullIndicatorPetrification;
            //}
        }
        else
        {
            Debug.LogError("No se ha asgnado la referencia a la calavera que muestra el cadáver a poseer. Nombre de cuerpo: " + name);
        }

    }

    protected override void Update()
    {
        base.Update();

        if (CanMove)
        {

            if (!IsGrounded())
            {
                //in the air while jumping
                if (!m_hasJumped && !m_coyoteTimeActive)
                {
                    m_coyoteTimeActive = true;
                    m_currentCoyoteTimer = m_coyoteTimeDuration;
                }
                else if (m_coyoteTimeActive)
                {
                    m_currentCoyoteTimer -= Time.deltaTime;
                    if (m_currentCoyoteTimer <= 0)
                    {
                        m_hasJumped = true;
                        m_coyoteTimeActive = false;
                    }
                }

                if (m_jumpHasBeenPressOnAir)
                {
                    m_currentTimerJumpOnAir = m_currentTimerJumpOnAir - Time.deltaTime;
                    if (m_currentTimerJumpOnAir <= 0)
                    {
                        m_jumpHasBeenPressOnAir = false;
                    }
                }


                //ascending part of the jump
                if (MyRgb.velocity.y > 1)
                {
                    if (m_isHoldingJump)
                    {
                        MyRgb.gravityScale = m_firstGravityHoldingJump;
                    }
                    else
                    {
                        //reset de velocidad en caso de dejar de pulsar el espacio durante el primer salto
                        if (!m_hasDoubleJumped)
                        {
                            //MyRgb.velocity = new Vector2(MyRgb.velocity.x, Mathf.Min(m_ySpeedWhenReleasingEarly, MyRgb.velocity.y));
                            //MyRgb.gravityScale = m_secondGravity;
                        }
                        MyRgb.gravityScale = m_firstGravity;
                    }

                }
                else if (MyRgb.velocity.y > 0)
                {
                    MyRgb.gravityScale = m_secondGravity;
                }
                else if (MyRgb.velocity.y > -1)
                {
                    if (m_hasDoubleJumped)
                    {
                        MyRgb.gravityScale = m_thirdGravityDoubleJump;
                    }
                    else
                    {
                        MyRgb.gravityScale = m_thirdGravity;
                    }
                }
                else
                {
                    MyRgb.gravityScale = m_fourthGravity;
                    if (MyRgb.velocity.y < -m_maxFallSpeedVertical)
                    {
                        MyRgb.velocity = new Vector2(MyRgb.velocity.x, -m_maxFallSpeedVertical);
                    }
                }

                ToggleWalkingParticles(false);
            }
            else
            {
                MyRgb.gravityScale = 2;
                m_coyoteTimeActive = false;

            }
        }
        m_myAnimator.SetFloat("xMovement", Mathf.Abs(MyRgb.velocity.x * 0.1f));

        m_myAnimator.SetFloat("yMovement", Mathf.Abs(MyRgb.velocity.y * 0.1f));


    }

    private void LateUpdate()
    {
        SkullIndicator();
        VerticalMovementOnLadder(InputManager.Instance.VerticalInputValue);
    }

    public void SkullIndicator()
    {
        if (m_skullIndicator && !InputManager.Instance.ThrowingHead)
        {            
            if (PossessionManager.Instance.MultiplePossessionWhenDead && !IsControlledByPlayer)
            {
                if (PossessionManager.Instance.DemonShowingSkull == this)
                    PossessionManager.Instance.DemonShowingSkull = null;
                if(PossessionManager.Instance.ControlledDemon)
                DistanceToPlayer = Vector2.Distance(m_torso.transform.position, PossessionManager.Instance.ControlledDemon.transform.position);
                if (PossessionManager.Instance.CheckBodyInRange(this) && !IsPossessionBlocked && !IsInDanger && IsDead && PossessionManager.Instance.ControlledDemon != null)
                {
                    m_skullIndicator.SetActive(true);
                }
                else
                {
                    if (PossessionManager.Instance.DemonsShowingSkulls.Contains(this))
                    {
                        PossessionManager.Instance.DemonsShowingSkulls.Remove(this);
                    }
                    m_skullIndicator.SetActive(false);
                }

            }
            else
            {
                if (PossessionManager.Instance.DemonShowingSkull == this && (IsControlledByPlayer || IsInDanger || IsPossessionBlocked))
                {
                    PossessionManager.Instance.DemonShowingSkull = null;
                }

                if (IsDead && PossessionManager.Instance.ControlledDemon != null && !IsInDanger && !PossessionManager.Instance.ControllingMultipleDemons && !IsPossessionBlocked)
                {
                    DistanceToPlayer = Vector2.Distance(m_torso.transform.position, PossessionManager.Instance.ControlledDemon.transform.position);
                    if (DistanceToPlayer <= PossessionManager.Instance.ControlledDemon.MaximumPossessionRange)
                    {
                        if (PossessionManager.Instance.DemonShowingSkull == null || PossessionManager.Instance.DemonShowingSkull == this || (PossessionManager.Instance.DemonShowingSkull != this && PossessionManager.Instance.DemonShowingSkull.DistanceToPlayer > DistanceToPlayer))
                        {
                            PossessionManager.Instance.DemonShowingSkull = this;
                            m_skullIndicator.SetActive(true);
                            m_skullIndicator.transform.position = m_torso.transform.position + Vector3.up * m_skullOffset;
                        }
                        else
                        {
                            m_skullIndicator.SetActive(false);
                        }

                    }
                    else
                    {

                        m_skullIndicator.SetActive(false);
                    }
                }
                else
                {
                    m_skullIndicator.SetActive(false);
                }
            }

        }
    }

    public void HideSkull()
    {
        //Debug.LogError("Hiding skull of: " + name);
        m_skullIndicator.SetActive(false);
    }
    public void ShowSkull()
    {
        m_skullIndicator.SetActive(true);
    }
    public void UnparentBodyParts(float explosionForce)
    {
        m_unparentedLimbs = true;
        //RagdollLogicCollider.gameObject.SetActive(false);
        IsPossessionBlocked = true;
        for (int i = 0; i < m_limbsToUnparent.Count; i++)
        {
            if(m_limbsToUnparent[i].transform.parent)
            m_limbsToUnparent[i].transform.parent = null;
            m_limbsToUnparent[i].GetComponent<HingeJoint2D>().enabled = false;
            m_limbsToUnparent[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(m_limbsToUnparent[i], m_timeToDestroyLimbsAfterUnparenting);
            if (explosionForce > 0)
                m_limbsToUnparent[i].GetComponent<Rigidbody2D>().AddForce((Vector2.up + Random.Range(-2, 2) * Vector2.right) * explosionForce, ForceMode2D.Impulse);
        }
        //m_spiritFire.SetActive(false);
        
        enabled = false;
    }
    public Transform UnparentLimbs()
    {
        m_unparentedLimbs = true;
        //RagdollLogicCollider.gameObject.SetActive(false);
        IsPossessionBlocked = true;
        for (int i = 0; i < m_limbsToUnparent.Count - 1; i++)
        {
            m_limbsToUnparent[i].transform.parent = null;
            m_limbsToUnparent[i].GetComponent<HingeJoint2D>().enabled = false;
            m_limbsToUnparent[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            Destroy(m_limbsToUnparent[i], m_timeToDestroyLimbsAfterUnparenting);
        }
        enabled = false;

        return Torso;
    }

    private void OnDisable()
    {
        m_skullIndicator.SetActive(false);
        if (PossessionManager.Instance.DemonShowingSkull == this)
            PossessionManager.Instance.DemonShowingSkull = null;
        else if (PossessionManager.Instance.DemonsShowingSkulls != null && PossessionManager.Instance.DemonsShowingSkulls.Contains(this))
        {
            Debug.LogError("a");
            PossessionManager.Instance.DemonsShowingSkulls.Remove(this);
        }
    }

    public void VerticalMovementOnLadder(float verticalInput)
    {
        if (m_isOnLadder)
        {
            MyRgb.gravityScale = 0f;
            MyRgb.velocity = new Vector2(MyRgb.velocity.x, verticalInput * MaxSpeed);
        }
        else if (verticalInput != 0)
        {
            m_tryingToGrabLadder = true;

        }
        else
        {
            m_tryingToGrabLadder = false;
        }
    }

    public bool CheckTraversePlatform()
    {
        if (IsGrounded())
        {
            RaycastHit2D impact = Physics2D.Raycast(transform.position,Vector2.down,1f,m_groundedDetectionLayers);
            if (impact.transform.CompareTag("Traversable"))
            {
                if (!m_tryingToTraversePlatform)
                {
                    //m_tryingToTraversePlatform = true;
                    //StartCoroutine(DelayResetInputTraversePlatform(m_waitTimeResetPlatformTraversal));
                }
                else
                {
                    TraversePlatform(impact.transform);
                    StopAllCoroutines();
                    m_tryingToTraversePlatform = false;
                    return true;
                }
            }
        }
        return false;
    }

    public void Slow(float m_slowPercentage)
    {
        m_maxSpeed *= (1 - m_slowPercentage * 0.01f);
        MyRgb.velocity = new Vector3(MyRgb.velocity.x, 0, 0);
        //Debug.LogError("Slowing, setting max speed of " + name + " to " + MaxSpeed);
    }
    public void CancelSlow(float m_slowPercentage)
    {
        m_maxSpeed /= (1 - m_slowPercentage * 0.01f);
        //Debug.LogError("Canceling slow, setting max speed of " + name + " to " + MaxSpeed);
    }

    IEnumerator DelayResetInputTraversePlatform(float time)
    {
        yield return new WaitForSeconds(time);
        m_tryingToTraversePlatform = false;
    }

    private void TraversePlatform(Transform platformToTraverse)
    {
        platformToTraverse.GetComponentInParent<TraversablePlatform>().Traverse();
    }

    public override void Move(float xInput)
    {
        float accel = m_acceleration;
        if (xInput == 0)
        {
            accel = m_deceleration;
        }
        else
        {
            if (IsGrounded())
            {
                if ((MyRgb.velocity.x) * xInput < 0)
                {
                    accel *= m_groundCorrectionMultiplier;
                }
            }
            else
            {
                accel *= m_airCorrectionMultiplier;
            }
        }


        //if(xInput != 0)
        //{
        //    if (!m_walkingParticles.isPlaying)
        //    {
        //        m_walkingParticles.Play();
        //    }
        //}

        MyRgb.velocity = new Vector2(Mathf.MoveTowards(MyRgb.velocity.x, xInput * MaxSpeed, accel * Time.deltaTime), MyRgb.velocity.y);
    }

    public override void Jump()
    {
        if (InputManager.Instance.VerticalInputValue < 0)
        {
            m_tryingToTraversePlatform = true;
            bool traversed = CheckTraversePlatform();
            if (traversed)
                return;
        }


        if (m_canJump)
        {
            if (m_jumpHasBeenPressOnAir)
            {
                m_jumpHasBeenPressOnAir = false;
            }
            if (!m_hasJumped || m_isOnLadder)
            {
                OnJumped?.Invoke();
                m_isJumping = true;
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * JumpForce);
                m_hasJumped = true;
                m_coyoteTimeActive = false;
                m_isHoldingJump = true;
                m_myAnimator.SetTrigger("Jump");
                AudioManager.Instance.PlayAudioSFX(m_jumpClip, false, 0.8f);
                AudioManager.Instance.PlayAudioSFX(m_jumpGruntClip, false, 2f);
                m_isOnLadder = false;
                
            }
            else if (m_canDoubleJump && !m_hasDoubleJumped)
            {
                OnJumped?.Invoke();
                MyRgb.velocity = new Vector2(MyRgb.velocity.x, 0);
                MyRgb.AddForce(Vector2.up * m_jumpForceSecond);
                m_hasDoubleJumped = true;
                m_myAnimator.SetTrigger("Jump");
                AudioManager.Instance.PlayAudioSFX(m_jumpClip, false, 0.8f);
                AudioManager.Instance.PlayAudioSFX(m_jumpGruntClip, false, 2f);
                
            }
            else if (m_hasJumped)
            {
                if (m_canDoubleJump && m_hasDoubleJumped)
                {
                    m_currentTimerJumpOnAir = m_jumpHasBeenPressOnAirTimer;
                    m_isHoldingJump = true;
                    m_jumpHasBeenPressOnAir = true;
                }
                else
                {
                    m_currentTimerJumpOnAir = m_jumpHasBeenPressOnAirTimer;
                    m_isHoldingJump = true;
                    m_jumpHasBeenPressOnAir = true;
                }
            }

        }
    }

    public override void JumpReleaseButton()
    {
        m_isHoldingJump = false;
    }

    public void ResetJumps()
    {
        m_hasJumped = false;
        m_hasDoubleJumped = false;
        m_isHoldingJump = false;
    }

    public void SetJumped()
    {
        m_hasJumped = true;
        m_hasDoubleJumped = false;
        MyRgb.gravityScale = m_secondGravity;
        m_isHoldingJump = false;
    }

    public override void ToggleWalkingParticles(bool active)
    {

        if (active)
        {
            if (!m_walkingParticles.isPlaying)
                m_walkingParticles.Play();
        }
        else
        {
            if (m_walkingParticles.isPlaying)
                m_walkingParticles.Stop();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (IsGrounded())
        {
            if (m_canJump)
            {
                if (m_hasJumped)
                {
                    AudioManager.Instance.PlayAudioSFX(m_landingClip, false, 0.25f);
                    m_jumpLandParticles.Play();
                    m_isJumping = false;
                }
                m_hasJumped = false;

                if (m_canDoubleJump)
                {
                    m_hasDoubleJumped = false;

                    if (m_jumpHasBeenPressOnAir && m_isHoldingJump)
                    {
                        Jump();
                        m_jumpHasBeenPressOnAir = false;
                    }
                }
                if (m_jumpHasBeenPressOnAir && m_isHoldingJump)
                {
                    Jump();
                    m_jumpHasBeenPressOnAir = false;
                }
            }
        }
        else
        {
            if (transform.parent != null)
            {
                SpawnerMatadero sm = GetComponentInParent<SpawnerMatadero>();
                if (sm != null)
                {
                    sm.DetachCharacter(this);
                }
            }

        }
    }

    public override void StopMovement()
    {
        MyRgb.velocity = Vector3.zero;
        m_previousGravityScale = MyRgb.gravityScale;
        MyRgb.gravityScale = 0;
    }

    public override void ContinueMovement()
    {
        MyRgb.gravityScale = m_previousGravityScale;
    }


    public void SetOnLadder(bool onLadder)
    {
        m_isOnLadder = onLadder;
        if (onLadder)
        {
            m_hasJumped = false;
            m_hasDoubleJumped = false;
            MyRgb.gravityScale = 0f;
        }
        m_myAnimator.SetBool("OnLadder", onLadder);
        m_faceCover.SetActive(onLadder);
    }

    public void ResetVelocity()
    {
        MyRgb.velocity = new Vector2(0, MyRgb.velocity.y);
    }

}
