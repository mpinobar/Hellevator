using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator m_bossAnimator;
    [SerializeField] Projectile m_knifePrefab;
    [SerializeField] Halo m_haloPrefab;
    [SerializeField] Color m_colorWhenHurt;
    [SerializeField] float m_haloSpawnHeight = 10f;
    [SerializeField] float m_knifeSpawnHeight = 10f;
    [SerializeField] float m_knifeSpeed = 40f;
    [SerializeField] float m_knifeDelay = 1f;
    [SerializeField] int m_maxHealth = 2;
    int m_currentHealth;
    [SerializeField] float m_offsetMovement = 5f;
    [SerializeField] float m_movementSpeed = 0.5f;
    [SerializeField] float m_maxCoordinate = -100f;
    [SerializeField] List<GameObject> m_limbsToUnparent;

    [SerializeField] List<GameObject> m_kitchenUtensils;
    List<Transform> m_kitchenUtensilsParents;
    List<Vector2> m_kitchenUtensilsStartingOffset;
    List<float> m_kitchenUtensilsStartingRot;

    [SerializeField] AnimationCurve m_visualKnivesHeightCurve;
    [SerializeField] float m_visualKnivesHeightMultiplier = 5f;
    [SerializeField] float m_visualKnivesUnparentDelay = 0.35f;
    [SerializeField] float m_visualKnivesDuration = 0.5f;

    [SerializeField] float m_explosionForce;
    [SerializeField] float m_timeFlickerWhenHurt = 0.08f;
    [SerializeField] float m_timeDelayToExplodeLimbs = 0.75f;
    [SerializeField] float m_timeDelayToDeactivate = 5f;
    Vector3 m_desiredPos;

	[Space]
	[SerializeField] GameObject m_bossKey = null;
	[SerializeField] Transform m_bossKeySpawnPoint = null;

    private enum State
    {
        Default, SeeingPlayer
    }

    [SerializeField] GameObject m_doorToCloseUponStart;
    [SerializeField] float m_timeToAttackPlayer = 2.5f;
    float m_playerSeenAttackTimer = 0f;

    bool m_started;

    bool m_attacking;

    State m_currentState = State.Default;


    private void Awake()
    {
        for (int i = 0; i < m_limbsToUnparent.Count; i++)
        {
            m_limbsToUnparent[i].GetComponent<Collider2D>().enabled = false;
            m_limbsToUnparent[i].GetComponent<Rigidbody2D>().isKinematic = true;
        }

        m_kitchenUtensilsParents = new List<Transform>();
        m_kitchenUtensilsStartingOffset = new List<Vector2>();

        for (int i = 0; i < m_kitchenUtensils.Count; i++)
        {
            m_kitchenUtensilsParents.Add(m_kitchenUtensils[i].transform.parent);
            m_kitchenUtensilsStartingOffset.Add(m_kitchenUtensils[i].transform.position - m_kitchenUtensilsParents[i].position);
        }

        m_playerSeenAttackTimer = 0f;
        m_doorToCloseUponStart.SetActive(false);
        m_bossAnimator = GetComponent<Animator>();
        m_currentHealth = m_maxHealth;
    }

    public void Begin()
    {
        m_started = true;
        SetNotSeeingPlayer();
        CloseEntrance();
        PossessionManager.Instance.Boss = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_started && PossessionManager.Instance.ControlledDemon != null)
        {
            if (m_currentState == State.SeeingPlayer)
            {
                m_playerSeenAttackTimer += Time.deltaTime;
                if (m_playerSeenAttackTimer >= m_timeToAttackPlayer)
                {
                    AttackPlayer();
                }
                float maxX = Mathf.Max(PossessionManager.Instance.ControlledDemon.transform.position.x - m_offsetMovement,m_maxCoordinate);
                m_desiredPos = new Vector3(maxX, transform.position.y, 0);
                transform.position = Vector3.Lerp(transform.position, m_desiredPos, Time.deltaTime * m_movementSpeed);
            }
            else
            {
                m_playerSeenAttackTimer = 0f;
            }
        }
    }

    public void SetSeeingPlayer()
    {
        m_currentState = State.SeeingPlayer;
    }

    public void SetNotSeeingPlayer()
    {
        m_currentState = State.Default;
        m_playerSeenAttackTimer = 0f;
    }

    public void DamageBoss()
    {
        m_currentHealth--;

        if (m_currentHealth > 0)
        {
            m_bossAnimator.SetTrigger("Hurting");

            StartCoroutine(HurtVisuals());
        }
        else
        {
            StartCoroutine(HurtVisuals());
            Die();
        }
    }

    public void ResetTimer()
    {
        m_playerSeenAttackTimer = 0;
    }

    private IEnumerator HurtVisuals()
    {
        SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();

        bool isRed = false;
        int switchCounter = 0;
        while (switchCounter <= 5)
        {

            for (int i = 0; i < childSprites.Length; i++)
            {
                if (isRed)
                {
                    childSprites[i].color = Color.white;

                }
                else
                {
                    childSprites[i].color = m_colorWhenHurt;

                }
            }
            isRed = !isRed;
            switchCounter++;
            yield return new WaitForSeconds(m_timeFlickerWhenHurt);
        }

    }

    private void Die()
    {
        PossessionManager.Instance.Boss = null;
        m_started = false;
        m_bossAnimator.SetTrigger("Death");
        Invoke(nameof(UnparentLimbs), m_timeDelayToExplodeLimbs);
        OpenEntrance();
        Invoke(nameof(DeactivateBoss), m_timeDelayToDeactivate);
    }

    private void DeactivateBoss()
    {
        gameObject.SetActive(false);
    }

    private void AttackPlayer()
    {
        //m_bossAnimator.SetTrigger("Attack");
        Halo halo = Instantiate(m_haloPrefab,PossessionManager.Instance.ControlledDemon.transform.position + Vector3.up*m_haloSpawnHeight,Quaternion.identity);
        halo.SetTarget(PossessionManager.Instance.ControlledDemon.transform, m_haloSpawnHeight);
        StartCoroutine(VisualKitchenKnives());
        ThrowKnife(PossessionManager.Instance.ControlledDemon.transform, m_knifeDelay, false);
        m_playerSeenAttackTimer = 0f;
    }

    public void ThrowKnife(Transform target, float delay, bool initial)
    {
        StartCoroutine(SpawnKnife(target, delay, initial));
    }
    IEnumerator SpawnKnife(Transform target, float delay, bool initial)
    {
        yield return new WaitForSeconds(delay);
        Projectile knife = Instantiate(m_knifePrefab, target.position + Vector3.up * m_knifeSpawnHeight, Quaternion.identity);
        m_playerSeenAttackTimer = 0f;
        knife.transform.localEulerAngles = Vector3.forward * 180;
        knife.Speed = m_knifeSpeed;
        knife.DestroyOnScenaryImpact = !initial;
    }
    public void CloseEntrance()
    {
        m_doorToCloseUponStart.SetActive(true);
    }

    private void OpenEntrance()
    {
        m_doorToCloseUponStart.SetActive(false);
		GameObject newBossKey = Instantiate(m_bossKey, m_bossKeySpawnPoint.position, Quaternion.identity);
    }

    public void UnparentLimbs()
    {
        Rigidbody2D cache = null;
        for (int i = 0; i < m_limbsToUnparent.Count; i++)
        {
            m_limbsToUnparent[i].transform.parent = null;
            m_limbsToUnparent[i].GetComponent<Collider2D>().enabled = true;
            cache = m_limbsToUnparent[i].GetComponent<Rigidbody2D>();
            cache.isKinematic = false;
            cache.velocity = Vector2.zero;
            cache.AddForce((Vector2.up + UnityEngine.Random.Range(-2, 2) * Vector2.right) * m_explosionForce, ForceMode2D.Impulse);
            //Destroy(m_limbsToUnparent[i].gameObject, 5f);
        }
    }
    private bool m_animatingKnives = false;
    private float time = 0f;
    private float evaluationTime = 0f;
    private void LateUpdate()
    {
        if (m_animatingKnives)
        {
            time += Time.deltaTime;
            evaluationTime += Time.deltaTime * m_visualKnivesHeightCurve.length / m_visualKnivesDuration;

            for (int i = 0; i < m_kitchenUtensils.Count; i++)
            {

                m_kitchenUtensils[i].transform.position = (Vector2)m_kitchenUtensilsParents[i].position + m_kitchenUtensilsStartingOffset[i] + Vector2.up * m_visualKnivesHeightCurve.Evaluate(evaluationTime) * m_visualKnivesHeightMultiplier;
                m_kitchenUtensils[i].transform.eulerAngles = Vector3.forward * m_kitchenUtensilsStartingRot[i];
            }
        }
    }
    public IEnumerator VisualKitchenKnives()
    {
        m_bossAnimator.SetTrigger("Attack");
        yield return new WaitForSeconds(m_visualKnivesUnparentDelay);



        if (m_kitchenUtensilsStartingRot == null)
        {
            m_kitchenUtensilsStartingRot = new List<float>();
        }
        m_kitchenUtensilsStartingRot.Clear();
        m_kitchenUtensilsStartingOffset.Clear();
        for (int i = 0; i < m_kitchenUtensils.Count; i++)
        {
            m_kitchenUtensilsStartingOffset.Add(m_kitchenUtensils[i].transform.position - m_kitchenUtensilsParents[i].position);
            m_kitchenUtensilsStartingRot.Add(m_kitchenUtensils[i].transform.eulerAngles.z);
        }
        UnparentKitchenKnives();
        m_animatingKnives = true;
        //GetComponent<Animator>().speed = 0f;
        //GetComponent<Animator>().enabled = false;
        time = 0f;
        evaluationTime = 0f;
        //while (time <= m_visualKnivesDuration)
        //{
        //    time += Time.deltaTime;
        //    evaluationTime += Time.deltaTime * m_visualKnivesHeightCurve.length / m_visualKnivesDuration;

        //    for (int i = 0; i < m_kitchenUtensils.Count; i++)
        //    {
        //        m_kitchenUtensils[i].transform.position = (Vector2)m_kitchenUtensilsParents[i].position + m_kitchenUtensilsStartingOffset[i] + Vector2.up * m_visualKnivesHeightCurve.Evaluate(evaluationTime) * m_visualKnivesHeightMultiplier;
        //    }

        //    yield return null;
        //}
        yield return new WaitForSeconds(m_visualKnivesDuration);

        m_animatingKnives = false;
        ReparentKitchenKnives();
        m_bossAnimator.ResetTrigger("Attack");
        //GetComponent<Animator>().enabled = true;        
        //GetComponent<Animator>().speed = 1f;
    }
    //public void VisualThrowKnives()
    //{
    //    for (int i = 0; i < m_kitchenUtensils.Count; i++)
    //    {
    //        m_kitchenUtensils[i].GetComponent<Rigidbody2D>().
    //    }
    //}

    private void UnparentKitchenKnives()
    {
        for (int i = 0; i < m_kitchenUtensils.Count; i++)
        {
            m_kitchenUtensils[i].transform.parent = null;
        }
    }

    private void ReparentKitchenKnives()
    {
        for (int i = 0; i < m_kitchenUtensils.Count; i++)
        {
            m_kitchenUtensils[i].transform.parent = m_kitchenUtensilsParents[i];
        }
    }

}
