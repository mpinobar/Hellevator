using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Satan : MonoBehaviour
{
    public enum Phase
    {
        First, Second, Third, Interphase
    }
    Phase m_phase = Phase.First;

    [Header("Attacks")]
    [SerializeField] float m_timeBetweenAttacks = 5f;
    float m_attackTimer;

    [SerializeField] HandAttackSatan m_verticalHandAttack;
    //[SerializeField] float m_verticalHandAttackDuration;
    [SerializeField] float m_verticalOffsetToSpawn;
    [SerializeField] HandAttackSatan m_horizontalHandAttack;
    //[SerializeField] float m_horizontalHandAttackDuration;

    [Header("Rayo")]
    [SerializeField] Rayo m_rayo;
    [SerializeField] float m_tiempoPreviewRayo = 1f;
    [SerializeField] float m_delayAparicionRayo = 1f;
    [SerializeField] Transform m_previewRayo;

    [Header("Phases")]
    [SerializeField] float m_firstPhaseYCoordinate;
    [SerializeField] Vector2 m_secondPhasePosition;
    [SerializeField] Vector2 m_thirdPhasePosition;
    [SerializeField] float m_disappearSpeed;

    [Space]
    [SerializeField] int m_maxLives;
    int m_currentLives;
    Animator m_anim;
    Vector2 m_bossPosition;
    public static Action OnInterphase;
    public static Action OnDeath;
    private void Start()
    {
        m_attackTimer = m_timeBetweenAttacks;
        m_anim = GetComponentInChildren<Animator>();
        m_previewRayo.gameObject.SetActive(false);
    }


    private void Update()
    {
        if (m_phase != Phase.Interphase && PossessionManager.Instance.ControlledDemon)
        {
            m_attackTimer -= Time.deltaTime;
            if (m_attackTimer < 0)
            {
                Attack();
            }
        }
        else
        {
            m_attackTimer = m_timeBetweenAttacks;
        }
    }
    public void SetPhase(Phase newPhase)
    {
        m_phase = newPhase;
        if (newPhase == Phase.Interphase)
            OnInterphase?.Invoke();
        else if(newPhase == Phase.Second || newPhase == Phase.Third)
        {
            transform.localScale = Vector3.one * 1.5f;
        }        
    }
    private void Attack()
    {
        float f = Random.value;
        if (f < 0.33f)
            /*StartCoroutine(*/
            VerticalHandAttack(/*PossessionManager.Instance.ControlledDemon.transform)*/);
        else if (f < 0.66f)
            /*StartCoroutine(*/
            HorizontalHandAttack(/*PossessionManager.Instance.ControlledDemon.transform)*/);
        else if (f <= 1)
            StartCoroutine(AtaqueRayo(PossessionManager.Instance.ControlledDemon.transform));
        m_attackTimer = m_timeBetweenAttacks;
    }

    private void LateUpdate()
    {
        if (PossessionManager.Instance.ControlledDemon)
        {            
            if (m_phase == Phase.First)
            {
                m_bossPosition = PossessionManager.Instance.ControlledDemon.transform.position;
                m_bossPosition.y = m_firstPhaseYCoordinate;
            }
            else if (m_phase == Phase.Second)
                m_bossPosition = m_secondPhasePosition;
            else if (m_phase == Phase.Third)
                m_bossPosition = m_thirdPhasePosition;
            else
                m_bossPosition.y -= m_disappearSpeed * Time.deltaTime;

            transform.position = m_bossPosition;
        }
    }

    public void ReceiveDamage()
    {
        m_anim.SetTrigger("Hurt");
        m_currentLives--;
        if(m_currentLives <= 0)
        {
            m_anim.SetTrigger("Death");
            OnDeath?.Invoke();
        }
    }

    void VerticalHandAttack(/*Transform target*/)
    {
        if (m_anim)
            m_anim.SetTrigger("Attack");
        m_verticalHandAttack.gameObject.SetActive(true);
        m_verticalHandAttack.transform.position = PossessionManager.Instance.ControlledDemon.transform.position + Vector3.up * m_verticalOffsetToSpawn;
        //yield return new WaitForSeconds(m_verticalHandAttackDuration);
        //m_verticalHandAttack.Enabled = false;
    }

    void HorizontalHandAttack(/*Transform target*/)
    {
        if (m_anim)
            m_anim.SetTrigger("Attack");
        m_horizontalHandAttack.gameObject.SetActive(true);
        //yield return new WaitForSeconds(m_horizontalHandAttackDuration);
        //m_horizontalHandAttack.Enabled = false;
    }
    IEnumerator AtaqueRayo(Transform target)
    {
        if (m_anim)
            m_anim.SetTrigger("Attack");
        m_rayo.transform.position = new Vector3(PossessionManager.Instance.ControlledDemon.transform.position.x, PossessionManager.Instance.ControlledDemon.transform.position.y + 14.3f, 0);
        m_rayo.gameObject.SetActive(false);
        if (m_previewRayo)
        {
            m_previewRayo.gameObject.SetActive(true);
            float t = 0;
            Vector2 posicionPreview = target.position;
            posicionPreview.y = m_previewRayo.position.y;
            while (t < m_tiempoPreviewRayo)
            {
                t += Time.deltaTime;
                posicionPreview.x = target.position.x;
                m_previewRayo.position = posicionPreview;
                yield return null;
            }
            yield return new WaitForSeconds(m_delayAparicionRayo);
        }
        m_rayo.transform.position = new Vector3(m_previewRayo.position.x, m_rayo.transform.position.y, 0);
        m_rayo.gameObject.SetActive(true);
        m_previewRayo.gameObject.SetActive(false);
    }

}
