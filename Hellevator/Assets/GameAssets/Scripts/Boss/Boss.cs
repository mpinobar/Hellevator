using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    Animator m_bossAnimator;
    [SerializeField] Projectile m_knifePrefab;
    [SerializeField] Color m_colorWhenHurt;
    [SerializeField] float m_knifeSpawnHeight = 10f;
    [SerializeField] float m_knifeSpeed = 40f;
    [SerializeField] int m_maxHealth = 2;
    int m_currentHealth;

    private enum State
    {
        Default, SeeingPlayer
    }

    [SerializeField] GameObject m_doorToCloseUponStart;
    [SerializeField] float m_timeUntilPlayerDeath;
    float m_playerSeenDeathTimer = 0f;

    bool m_started;

    bool m_attacking;

    State m_currentState = State.Default;

    private void Awake()
    {

        m_playerSeenDeathTimer = 0f;
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
                m_playerSeenDeathTimer += Time.deltaTime;
                if (m_playerSeenDeathTimer >= m_timeUntilPlayerDeath)
                {
                    AttackPlayer();
                }
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
        m_playerSeenDeathTimer = 0f;
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
        m_playerSeenDeathTimer = 0;
    }

    private IEnumerator HurtVisuals()
    {        
        SpriteRenderer[] childSprites = GetComponentsInChildren<SpriteRenderer>();

        bool isRed = false;
        int switchCounter = 0;
        while(switchCounter <= 5)
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
            yield return new WaitForSeconds(0.08f);
        }

    }

    private void Die()
    {
        PossessionManager.Instance.Boss = null;
        m_started = false;
        m_bossAnimator.SetTrigger("Death");
        Invoke(nameof(DeactivateBoss), 2f);
    }

    private void DeactivateBoss()
    {
        gameObject.SetActive(false);
        OpenEntrance();
    }

    private void AttackPlayer()
    {
        m_bossAnimator.SetTrigger("Attack");
        ThrowKnife(PossessionManager.Instance.ControlledDemon.transform, 0, false);
        m_playerSeenDeathTimer = 0f;
    }

    public void ThrowKnife(Transform target, float delay, bool initial)
    {
        StartCoroutine(SpawnKnife(target, delay, initial));
    }
    IEnumerator SpawnKnife(Transform target, float delay, bool initial)
    {
        yield return new WaitForSeconds(delay);
        Projectile knife = Instantiate(m_knifePrefab, target.position + Vector3.up * m_knifeSpawnHeight, Quaternion.identity);
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
        m_doorToCloseUponStart.SetActive(true);        
    }
}
