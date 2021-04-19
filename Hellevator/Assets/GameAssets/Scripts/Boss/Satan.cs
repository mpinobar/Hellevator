using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class Satan : MonoBehaviour
{
    private enum Phase
    {
        First, Second, Third, Interphase
    }
    Phase m_phase = Phase.First;

    [Header("Attacks")]
    [SerializeField] float m_timeBetweenAttacks = 5f;
    float m_attackTimer;

    [SerializeField] HandAttackSatan m_verticalHandAttack;
    [SerializeField] float m_verticalHandAttackDuration;
    [SerializeField] float m_verticalOffsetToSpawn;
    [SerializeField] HandAttackSatan m_horizontalHandAttack;
    [SerializeField] float m_horizontalHandAttackDuration;

    [Header("Rayo")]
    [SerializeField] Rayo m_rayo;
    [SerializeField] float m_tiempoPreviewRayo = 1f;
    [SerializeField] float m_delayAparicionRayo = 1f;
    [SerializeField] Transform m_previewRayo;

    [Header("Phases")]
    [SerializeField] float m_firstPhaseYCoordinate;
    [SerializeField] float m_secondPhaseYCoordinate;
    [SerializeField] float m_thirdPhaseYCoordinate;
    [SerializeField] float m_disappearSpeed;

    Animator m_anim;
    Vector2 m_bossPosition;

    private void Start()
    {
        m_attackTimer = m_timeBetweenAttacks;
        m_anim = GetComponentInChildren<Animator>();
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

    private void Attack()
    {
        float f = Random.value;
        if (f < 0.33f)
            StartCoroutine(VerticalHandAttack(PossessionManager.Instance.ControlledDemon.transform)); 
        else if (f < 0.66f)
            StartCoroutine(HorizontalHandAttack(PossessionManager.Instance.ControlledDemon.transform));
        else if (f <= 1)
            StartCoroutine(AtaqueRayo(PossessionManager.Instance.ControlledDemon.transform));
        m_attackTimer = m_timeBetweenAttacks;
    }

    private void LateUpdate()
    {
        if (PossessionManager.Instance.ControlledDemon)
        {
            m_bossPosition = PossessionManager.Instance.ControlledDemon.transform.position;
            if (m_phase == Phase.First)
                m_bossPosition.y = m_firstPhaseYCoordinate;
            else if (m_phase == Phase.Second)
                m_bossPosition.y = m_secondPhaseYCoordinate;
            else if (m_phase == Phase.Third)
                m_bossPosition.y = m_thirdPhaseYCoordinate;
            else
                m_bossPosition.y -= m_disappearSpeed * Time.deltaTime;

            transform.position = m_bossPosition;
        }
    }

    IEnumerator VerticalHandAttack(Transform target)
    {
        m_anim.SetTrigger("VerticalAttack");
        m_verticalHandAttack.Enabled = true;
        m_verticalHandAttack.transform.position = PossessionManager.Instance.ControlledDemon.transform.position + Vector3.up * m_verticalOffsetToSpawn;
        yield return new WaitForSeconds(m_verticalHandAttackDuration);
        m_verticalHandAttack.Enabled = false;
    }

    IEnumerator HorizontalHandAttack(Transform target)
    {
        m_anim.SetTrigger("HorizontalAttack");
        m_horizontalHandAttack.Enabled = true;
        yield return new WaitForSeconds(m_horizontalHandAttackDuration);
        m_horizontalHandAttack.Enabled = false;
    }
    IEnumerator AtaqueRayo(Transform target)
    {
        m_anim.SetTrigger("Rayo");
        m_rayo.gameObject.SetActive(false);
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

        m_rayo.gameObject.SetActive(true);
    }   

}
