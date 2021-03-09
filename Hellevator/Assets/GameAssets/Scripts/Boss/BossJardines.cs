using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJardines : MonoBehaviour
{
    [SerializeField] int m_maxLives = 3;
    [SerializeField] float m_delayToSpawn = 2f;
    [SerializeField] float m_swimmingTime = 15f;
    float m_swimmingTimer = 15f;
    [SerializeField] float m_surfaceTime = 15f;
    [SerializeField] float m_swimmingSpeed = 5f;
    //float m_surfaceTimer = 15f;

    [SerializeField] AmphibianDemon m_prefabAnifibio = null;
    [SerializeField] AmphibianDemon m_spawnedDemon;
    [SerializeField] Transform m_spawnTransformAnfibio = null;
    [SerializeField] Transform m_pointToJumpToSwim = null;
    [SerializeField] Transform m_swimmingEndPoint = null;
    Vector3 m_initialPosition;
    Animator m_animator;

    // Start is called before the first frame update
    void Start()
    {
        m_initialPosition = transform.position;
        m_animator = GetComponent<Animator>();
        if (!m_spawnedDemon)
            m_spawnedDemon = Instantiate(m_prefabAnifibio);
        m_spawnedDemon.gameObject.SetActive(false);
        //m_swimmingTimer = m_swimmingTime;
        //m_surfaceTime = m_surfaceTimer;
    }


    public void StartCombat()
    {
        StartCoroutine(SurfaceBehavior());
    }

    IEnumerator SurfaceBehavior()
    {
        Debug.LogError("Starting surface behavior");
        yield return new WaitForSeconds(m_delayToSpawn);
        Debug.LogError("Spawning amphibian demon");
        //m_animator.SetTrigger("AVISO DE SPAWNEAR ANFIBIO");
        
        m_spawnedDemon.gameObject.SetActive(true);
        m_spawnedDemon.ReturnToPatrol();
        m_spawnedDemon.transform.position = m_spawnTransformAnfibio.position;
        yield return new WaitForSeconds(m_surfaceTime);
        Debug.LogError("GOING INTO WATER");
        //m_animator.SetTrigger("AVISO DE IR AL AGUA");
        StartCoroutine(BelowWaterBehavior());
    }

    IEnumerator BelowWaterBehavior()
    {
        m_spawnedDemon.StopAllCoroutines();
        m_spawnedDemon.gameObject.SetActive(false);
        transform.position = m_pointToJumpToSwim.position;
        Debug.LogError("Jumped into water, starting to swim");
        m_swimmingTimer = m_swimmingTime;
        Transform target = m_swimmingEndPoint;
        while (m_swimmingTimer > 0)
        {
            m_swimmingTimer -= Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, target.position, m_swimmingSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target.position) <= 0.5f)
            {
                if(target == m_swimmingEndPoint)
                {
                    target = m_pointToJumpToSwim;
                }
                else
                {
                    target = m_swimmingEndPoint;
                }
            }
            yield return null;
        }

        //Debug.LogError("END OF WATER BEHAVIOR");
        //m_animator.SetTrigger("SALIR DE AGUA TRIGGER");
        transform.position = m_initialPosition;
        StartCoroutine(SurfaceBehavior());
    }


}
