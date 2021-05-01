using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caldero : MonoBehaviour
{
    [SerializeField] SpriteRenderer m_spr;
    [SerializeField] float m_timeToExplode = 5f;
    float m_explodeTimer;
    [SerializeField] GameObject m_explosionPrefab;
    private bool m_covered = false;
    float m_changeSpeed;
    [SerializeField] Fire m_cauldronFire;
    bool canExplode = true;
    private void Awake()
    {
        m_changeSpeed = 1 / m_timeToExplode;
    }
    bool coroutineActive;
    private void Update()
    {
        m_covered = m_cauldronFire.m_fireCovered;
        if (m_covered)
        {
            if (!coroutineActive)
            {
                coroutineActive = true;
                StartCoroutine(ShakeCauldron());
            }
            if (!canExplode)
            {
                m_covered = false;
                StopAllCoroutines();
            }
        }
        if (m_covered)
        {
            if (m_explodeTimer < m_timeToExplode)
                m_explodeTimer += Time.deltaTime;
            else if (canExplode)
                Explode();
            //m_spr.color = Color.Lerp(m_spr.color, Color.red, Time.deltaTime * m_changeSpeed);
        }
        else
        {
            if (m_explodeTimer > 0)
                m_explodeTimer -= Time.deltaTime;
            else
                canExplode = true;
            // m_spr.color = Color.Lerp(m_spr.color, Color.white, Time.deltaTime * m_changeSpeed);
        }
    }



    public void Explode()
    {
        GameObject go = Instantiate(m_explosionPrefab, transform.position, Quaternion.identity);
        go.transform.localScale = Vector3.one * 10f;
        canExplode = false;
        coroutineActive = false;
    }

    IEnumerator ShakeCauldron()
    {
        Transform cldr = m_spr.transform;
        int framesPerShake = 1;
        float frames = 0;
        float shakeIntensity = 3f;
        Vector2 shakeDir = new Vector2(Random.value,Random.value)*shakeIntensity * (m_explodeTimer/m_timeToExplode);
        Vector2 initPos = cldr.position;
        while (m_explodeTimer < m_timeToExplode)
        {
            cldr.position = Vector2.Lerp(cldr.position, initPos + shakeDir, 3 * Time.deltaTime);
            frames++;
            if(frames >= framesPerShake)
            {
                shakeDir = new Vector2(Random.value*2-1, Random.value * 2 - 1) * shakeIntensity * (m_explodeTimer / m_timeToExplode);
                frames = 0;
            }
            yield return null;
        }
        cldr.position = initPos;
    }
}
