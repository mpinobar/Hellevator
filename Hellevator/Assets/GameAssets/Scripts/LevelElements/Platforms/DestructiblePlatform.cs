using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructiblePlatform : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_decorado;
    [SerializeField] private float m_timeToReappear = 2f;
    [SerializeField] private float m_timeToDestroy;
    [SerializeField] private float m_shakeStrength;
    [SerializeField] private float m_multiplierAtEnd = 2;
    [SerializeField] private int m_framesPerShake;
    SpriteRenderer rnd;
    private bool m_destroying;
    private float m_currentMultiplier = 1;
    private Vector3 m_initPos;
    private float tmp;
    private float m_percentageOfDestruction;
    Collider2D m_collider;
    private float tmpReappear;
    private int tmpShake;
    private Vector2 m_targetShakePosition;
    private bool m_willReappear = true;
    private bool m_turnsKinematicOnSpikesEnter;
    private bool m_turnsKinematicOnCollisionEnter;

    LayerMask m_playerLayer;
    LayerMask m_bodyLayer;

    List<SpikesWeightData> m_spikesData;
    private List<DemonBase> m_enemiesOnPreassurePlate;

	[SerializeField] AudioClip m_dissapearingClip;

    public bool WillReappear { get => m_willReappear; set => m_willReappear = value; }
    public bool TurnsKinematicOnSpikesEnter { get => m_turnsKinematicOnSpikesEnter; set => m_turnsKinematicOnSpikesEnter = value; }
	public bool TurnsKinematicOnCollisionEnter { get => m_turnsKinematicOnCollisionEnter; set => m_turnsKinematicOnCollisionEnter = value; }

	private bool m_isParentMovingPlatform;

    Dissolve m_dissolveCmp;

    Rigidbody2D m_rgb;
    // Start is called before the first frame update
    void Start()
    {
        m_dissolveCmp = GetComponentInChildren<Dissolve>();
        m_collider = GetComponent<Collider2D>();
        rnd = GetComponent<SpriteRenderer>();
        m_rgb = GetComponent<Rigidbody2D>();
        if (m_decorado)
        {
            m_initPos = m_decorado.transform.position;
        }
        tmp = m_timeToDestroy;
        tmpShake = m_framesPerShake;
        tmpReappear = m_timeToReappear;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_destroying)
        {
            tmp -= Time.deltaTime;
            m_percentageOfDestruction = tmp / m_timeToDestroy;
            Color c = rnd.color;
            c.a = m_percentageOfDestruction;
            rnd.color = c;
            if (m_decorado)
            {
                m_decorado.color = c;
                m_currentMultiplier = (1 - m_percentageOfDestruction) * m_multiplierAtEnd;
                if (tmpShake == m_framesPerShake)
                {
                    m_targetShakePosition = new Vector2(m_initPos.x + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f * m_currentMultiplier, m_initPos.y + Random.Range(-m_shakeStrength, m_shakeStrength) * 0.1f * m_currentMultiplier);
                }
                tmpShake--;
                m_decorado.transform.position = Vector2.MoveTowards(m_decorado.transform.position, m_targetShakePosition, m_currentMultiplier * Time.deltaTime);
                if (tmpShake <= 0)
                {
                    tmpShake = m_framesPerShake;
                }

            }
            if (tmp <= 0)
            {
                m_collider.enabled = false;
                m_destroying = false;
            }
        }
        else if (m_willReappear)
        {
            if (tmp <= 0)
            {
                tmpReappear -= Time.deltaTime;
                if (tmpReappear <= 0)
                {
                    tmp = m_timeToDestroy;
                    tmpReappear = m_timeToReappear;
                    m_collider.enabled = true;
                    Color c = rnd.color;
                    c.a = 1;
                    rnd.color = c;
                    if (m_decorado)
                    {
                        m_decorado.color = c;
                        m_decorado.transform.position = m_initPos;
                    }
                }
            }
        }
    }

    private void CallReverseDissolve()
    {
        m_dissolveCmp.StartReverseDissolve();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        DemonBase demon = collision.transform.root.GetComponent<DemonBase>();
        if (demon && demon.IsControlledByPlayer)
        {
            RaycastHit2D hit = Physics2D.Raycast(collision.transform.position,Vector2.down,0.5f,1 << 0);
            
            if (hit.transform != null && hit.transform == transform)
            {
                m_destroying = true;
                m_dissolveCmp?.NormalizeValues(1/m_timeToDestroy);
                m_dissolveCmp?.StartDissolve();
                Invoke(nameof(CallReverseDissolve), m_timeToReappear + m_timeToDestroy);
				AudioManager.Instance.PlayAudioSFX(m_dissapearingClip, false, 1f);
            }
        }
        else
        {
            if (!m_willReappear)
            {                
				if(!m_rgb.isKinematic)
				{
                    m_rgb.isKinematic = m_turnsKinematicOnCollisionEnter;
					if (m_rgb.isKinematic)
					{  
                        m_rgb.constraints = RigidbodyConstraints2D.FreezePosition;
					}
					else
					{
                        m_rgb.constraints = RigidbodyConstraints2D.FreezePositionX;				
					}
                    m_rgb.constraints = RigidbodyConstraints2D.FreezeRotation;
				}
				HorizontalPeriodicPlatform hpp = collision.transform.GetComponent<HorizontalPeriodicPlatform>();
                if (hpp)
                {                   
                    m_bodyLayer = hpp.BodyLayer;
                    m_playerLayer = hpp.PlayerLayer;
                    transform.parent = collision.transform;
                    m_isParentMovingPlatform = true;
                }
            }
        }


        if (m_isParentMovingPlatform)
        {
            if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
                return;
            DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
            //Debug.LogError(cmpDemon.name);
            if (cmpDemon != null)
            {
                RaycastHit2D hit = Physics2D.Raycast(cmpDemon.Torso.position, Vector2.down, 2f, 1<<0);
                if (hit.transform != null && (hit.transform == transform || hit.transform == transform.GetChild(0)))
                {
                    
                    m_enemiesOnPreassurePlate.Add(cmpDemon);
                    cmpDemon.transform.parent = transform;

                    //}
                }


            }
        }

    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (m_isParentMovingPlatform)
        {
            if (LayerMask.LayerToName(collision.gameObject.layer) != "Player" && LayerMask.LayerToName(collision.gameObject.layer) != "Body")
                return;

            DemonBase cmpDemon = collision.transform.GetComponentInParent<DemonBase>();
            if (cmpDemon != null)
            {

                if (m_enemiesOnPreassurePlate.Contains(cmpDemon) /*cmpDemon.IsControlledByPlayer*/)
                {
                    m_enemiesOnPreassurePlate.Remove(cmpDemon);
                    cmpDemon.transform.parent = null;
                    return;
                }
            }
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_turnsKinematicOnSpikesEnter)
        {
            if(collision.GetComponent<Spikes>() != null)
            {
                m_rgb.isKinematic = true;
				//print("Hello");
                if (!m_willReappear)
                {
                    m_rgb.isKinematic = true;
                    m_rgb.constraints = RigidbodyConstraints2D.FreezePosition;
                    HorizontalPeriodicPlatform hpp = collision.transform.GetComponentInParent<HorizontalPeriodicPlatform>();
                    if (hpp)
                    {
                        m_bodyLayer = hpp.BodyLayer;
                        m_playerLayer = hpp.PlayerLayer;
                        transform.parent = collision.transform;
                        m_isParentMovingPlatform = true;
                    }
                }
            }
        }
    }
}
