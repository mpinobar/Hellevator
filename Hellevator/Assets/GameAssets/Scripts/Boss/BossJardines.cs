using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossJardines : MonoBehaviour
{
    [SerializeField] int m_maxLives = 3;
    [SerializeField] float m_delayToSpawn = 2f;
    [SerializeField] float m_swimmingTime = 15f;
    float m_swimmingTimer = 15f;
    [SerializeField] float m_surfaceTimeBeforeJumping = 15f;
    [SerializeField] float m_swimmingSpeed = 5f;
    //float m_surfaceTimer = 15f;

    [SerializeField] AmphibianDemon m_prefabAnifibio = null;
    [SerializeField] AmphibianDemon m_spawnedDemon;
    [SerializeField] Transform m_spawnTransformAnfibio = null;
    [SerializeField] Transform m_pointToJumpToSwim = null;
    [SerializeField] Transform m_swimmingEndPoint = null;
    Vector3 m_initialPosition;
    Animator m_animator;
    [Space]
    [SerializeField] AnimationCurve m_xMovementJumpToWater;
    [SerializeField] AnimationCurve m_yMovementJumpToWater;
    [SerializeField] float m_jumpToWaterSpeed;
    [SerializeField] float m_animationDelayBeforeJumping;
    [SerializeField] float m_animationDelayBeforeSwimming;
    [Space]
    [SerializeField] AnimationCurve m_xMovementJumpOutOfWater;
    [SerializeField] AnimationCurve m_yMovementJumpOutOfWater;
    [SerializeField] float m_jumpOutOfWaterSpeed;
    [Space]

    bool m_canGetHurt;
    [SerializeField] int m_lives = 3;

    [SerializeField] SpriteRenderer m_button;
    [SerializeField] Material m_materialWhenInactive;
    [SerializeField] Material m_materialWhenActive;
    [SerializeField] GameObject m_activateOnDeath;
    [SerializeField] GameObject m_deactivateOnDeath;
    Glow glowCMP;
    public bool CanGetHurt
    {
        get => m_canGetHurt;
        set
        {
            if (value)
            {
                m_button.material = m_materialWhenActive;
                glowCMP.enabled = true;
            }
            else
            {
                m_button.material = m_materialWhenInactive;
                glowCMP.enabled = false;
            }
            m_canGetHurt = value;
        }
    }
    private void Awake()
    {
        glowCMP = m_button.GetComponent<Glow>();
        glowCMP.enabled = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        flotadores[0].GetComponent<SpriteRenderer>().material = redGlowMaterial;
        m_initialPosition = transform.position;
        m_animator = GetComponentInChildren<Animator>();
        if (!m_spawnedDemon)
            m_spawnedDemon = Instantiate(m_prefabAnifibio);
        m_spawnedDemon.gameObject.SetActive(false);
        //m_swimmingTimer = m_swimmingTime;
        //m_surfaceTime = m_surfaceTimer;
    }


    public void StartCombat()
    {
        //StartCoroutine(SurfaceBehavior());
        StartCoroutine(BelowWaterBehavior());
    }

    IEnumerator SurfaceBehavior()
    {
        transform.GetChild(0).localScale = Vector3.one;
        transform.GetChild(0).localEulerAngles = Vector3.forward * 5;
        transform.GetChild(0).localPosition = new Vector3(6.12f, 1.13f, 0);
        yield return new WaitForSeconds(3);
        int numSpawns = 1;

        while (numSpawns > 0)
        {
            m_animator.SetTrigger("spawnDemon");
            yield return new WaitForSeconds(1);
            numSpawns--;
            m_spawnedDemon.gameObject.SetActive(true);
            m_spawnedDemon.transform.position = m_spawnTransformAnfibio.position;
            m_spawnedDemon.ReturnToPatrol();
            yield return new WaitForSeconds(m_delayToSpawn);
        }

        //m_spawnedDemon.gameObject.SetActive(true);
        //m_spawnedDemon.ReturnToPatrol();
        //m_spawnedDemon.transform.position = m_spawnTransformAnfibio.position;
        yield return new WaitForSeconds(m_surfaceTimeBeforeJumping);
        StartCoroutine(BelowWaterBehavior());
    }


    Transform target;
    IEnumerator BelowWaterBehavior()
    {
        m_spawnedDemon.StopAllCoroutines();
        m_spawnedDemon.gameObject.SetActive(false);
        m_animator.SetBool("swimming", true);
        yield return StartCoroutine(JumpThroughAnimationCurve(transform, m_initialPosition, m_pointToJumpToSwim.position, m_xMovementJumpToWater, m_yMovementJumpToWater, m_jumpToWaterSpeed, m_animationDelayBeforeJumping));
        CanGetHurt = true;
        m_swimmingTimer = m_swimmingTime;
        target = m_swimmingEndPoint;
        Vector3 delayedSwimmingEndpoint = m_swimmingEndPoint.position;
        delayedSwimmingEndpoint.x = m_pointToJumpToSwim.position.x + 3;
        //StartCoroutine(RotationCoroutine(m_animator.transform.parent, m_animator.transform.parent.localEulerAngles.z, -8, 1));

        transform.GetChild(0).localScale = Vector3.one;
        transform.GetChild(0).localEulerAngles = Vector3.forward * -6.5f;
        transform.GetChild(0).localPosition = new Vector3(6.12f, 1.13f, 0);

        while (m_swimmingTimer > 0 && m_canGetHurt)
        {
            m_swimmingTimer -= Time.deltaTime;
            if (m_swimmingTimer < (m_swimmingTime - m_animationDelayBeforeSwimming))
                transform.position = Vector3.MoveTowards(transform.position, target.position, m_swimmingSpeed * Time.deltaTime);
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, delayedSwimmingEndpoint, m_swimmingSpeed * Time.deltaTime);
                transform.position += Vector3.up * Time.deltaTime * 0.5f;
            }
            if (Vector3.Distance(transform.position, target.position) <= 0.5f)
            {
                if (target == m_swimmingEndPoint)
                {
                    target = m_pointToJumpToSwim;
                    transform.GetChild(0).localScale = Vector3.one - Vector3.right * 2;
                    transform.GetChild(0).localEulerAngles = Vector3.forward * 12;
                    transform.GetChild(0).localPosition = new Vector3(-8.25f, 1.13f, 0);
                }
                else
                {
                    target = m_swimmingEndPoint;
                    transform.GetChild(0).localScale = Vector3.one;
                    transform.GetChild(0).localEulerAngles = Vector3.forward * -6.5f;
                    transform.GetChild(0).localPosition = new Vector3(6.12f, 1.13f, 0);
                }
            }
            yield return null;
        }
        transform.GetChild(0).localScale = Vector3.one - Vector3.right * 2;
        transform.GetChild(0).localEulerAngles = Vector3.forward * 12;
        transform.GetChild(0).localPosition = new Vector3(-8.25f, 1.13f, 0);
        target = m_pointToJumpToSwim;
        while (Vector3.Distance(transform.position, target.position) >= 0.5f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, m_swimmingSpeed * Time.deltaTime);
            yield return null;
        }

        CanGetHurt = false;
        m_animator.SetBool("swimming", false);
        yield return StartCoroutine(JumpThroughAnimationCurve(transform, m_pointToJumpToSwim.position, m_initialPosition, m_xMovementJumpOutOfWater, m_yMovementJumpOutOfWater, m_jumpOutOfWaterSpeed));
        //StartCoroutine(RotationCoroutine(m_animator.transform.parent, m_animator.transform.parent.localEulerAngles.z, 0, 1));
        StartCoroutine(SurfaceBehavior());
    }

    IEnumerator JumpThroughAnimationCurve(Transform characterToMove, Vector3 startPos, Vector3 endPos, AnimationCurve xMovement, AnimationCurve yMovement, float animationSpeed, float initialWaitTime = 0f)
    {
        yield return new WaitForSeconds(initialWaitTime);
        float iterator = 0f;
        characterToMove.position = startPos;
        Vector3 nextPosition = startPos;
        while (iterator < 1)
        {
            iterator += Time.deltaTime * animationSpeed;
            nextPosition.x = startPos.x + (endPos.x - startPos.x) * xMovement.Evaluate(iterator);
            nextPosition.y = startPos.y + (endPos.y - startPos.y) * yMovement.Evaluate(iterator);
            characterToMove.position = nextPosition;
            yield return null;
        }
    }

    IEnumerator RotationCoroutine(Transform character, float start, float end, float speed)
    {
        Vector3 rotation = new Vector3(0,0,start);
        character.localEulerAngles = rotation;
        float t = 1;
        while (t > 0)
        {
            t -= Time.deltaTime * speed;
            rotation.z = Mathf.Lerp(rotation.z, end, speed * Time.deltaTime);
            character.localEulerAngles = rotation;
            yield return null;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (CanGetHurt)
        {
            collision.transform.TryGetComponent(out DemonBase player);
            if (player && player.IsControlledByPlayer)
            {
                ((BasicZombie)player).ResetJumps();
                CanGetHurt = false;
                TakeDamage();
                player.MyRgb.velocity = new Vector2(player.MyRgb.velocity.x, 0);
                player.MyRgb.AddForce(Vector2.up * ((BasicZombie)player).JumpForce * 1.5f);
            }
        }
    }

    private void TakeDamage()
    {
        m_lives--;
        if (m_lives <= 0)
        {
            StopAllCoroutines();
            m_animator.SetTrigger("dead");
            StartCoroutine(Sink());
            if (m_activateOnDeath) { 
                m_activateOnDeath.SetActive(true);
                m_deactivateOnDeath.SetActive(false);
            }
            flotadores[0].parent = null;
            flotadores[0].GetComponent<Rigidbody2D>().isKinematic = false;            
            flotadores.RemoveAt(0);            
            AchievementsManager.UnlockKilledGK();
        }
        else
        {
            flotadores[0].parent = null;
            flotadores[0].GetComponent<Rigidbody2D>().isKinematic = false;
            flotadores.RemoveAt(0);
            flotadores[0].GetComponent<SpriteRenderer>().material = redGlowMaterial;
            m_animator.SetTrigger("hurt");
            target = m_pointToJumpToSwim;
            transform.GetChild(0).localScale = Vector3.one - Vector3.right * 2;
            transform.GetChild(0).localEulerAngles = Vector3.forward * 12;
            transform.GetChild(0).localPosition = new Vector3(-8.25f, 1.13f, 0);
        }
    }
    [SerializeField] List<Transform> flotadores;
    [SerializeField] Material redGlowMaterial;
    IEnumerator Sink()
    {
        while (true)
        {
            transform.position += Vector3.down * Time.deltaTime * 5f;
            yield return null;
        }
    }
}
