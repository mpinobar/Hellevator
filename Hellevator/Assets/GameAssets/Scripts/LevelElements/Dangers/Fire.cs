using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Fire : MonoBehaviour
{
    Vector2 m_startingPoint;
    Vector2 m_endPoint;
    float m_height;
    SpriteRenderer m_spriteRenderer;
    MaterialPropertyBlock m_propertyBlock;
    float m_currentFireAltitude = 1;
    Vector2 m_size;
    [SerializeField] LayerMask m_detectionLayer;
    RaycastHit2D[] m_impacts;

    [SerializeField] AudioClip m_bodyBurningClip;
    bool m_hasPlayedAudio;
    List<GameObject> m_detections;
    public bool m_fireCovered;
    // Start is called before the first frame update
    void Start()
    {
        m_height = transform.localScale.x;
        m_startingPoint = transform.position - Vector3.up * m_height * 0.5f;
        m_endPoint = transform.position + Vector3.up * m_height * 0.5f;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_propertyBlock = new MaterialPropertyBlock();
        m_spriteRenderer.GetPropertyBlock(m_propertyBlock);
        m_size = new Vector2(transform.localScale.y * 0.75f, 1);
        m_detections = new List<GameObject>();        
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        //Debug.DrawRay(m_startingPoint, (m_endPoint - m_startingPoint) * m_currentFireAltitude, Color.green);
        //Debug.DrawRay(m_startingPoint - (m_size * 0.5f) * Vector2.right, (m_endPoint - m_startingPoint) * m_currentFireAltitude, Color.green);
        //Debug.DrawRay(m_startingPoint + (m_size * 0.5f) * Vector2.right, (m_endPoint - m_startingPoint) * m_currentFireAltitude, Color.green);
#endif
        m_impacts = Physics2D.BoxCastAll(m_startingPoint, m_size, 0, m_endPoint - m_startingPoint, m_height * m_currentFireAltitude, m_detectionLayer);        
        if (m_impacts.Length > 0)
        {
            for (int i = 0; i < m_impacts.Length; i++)
            {
                if (m_impacts[i].transform != null && m_detections.Contains(m_impacts[i].transform.gameObject))
                {
                    if (LayerMask.LayerToName(m_impacts[i].transform.gameObject.layer) == "Player")
                    {
                        m_impacts[i].transform.GetComponent<DemonBase>().Die(false);
                        if (!m_hasPlayedAudio)
                        {
                            StartCoroutine(ResetPlayedAudio(1));
                            AudioManager.Instance.PlayAudioSFX(m_bodyBurningClip, false, 1f);
                            m_hasPlayedAudio = true;
                        }
                        Destroy(m_impacts[i].transform.gameObject, 1f);
                    }
                    else if (LayerMask.LayerToName(m_impacts[i].transform.gameObject.layer) == "Body")
                    {
                        if (m_impacts[i].transform.GetComponentInParent<DemonBase>())
                        {
                            if (!m_hasPlayedAudio)
                            {
                                StartCoroutine(ResetPlayedAudio(1));
                                AudioManager.Instance.PlayAudioSFX(m_bodyBurningClip, false, 1f);
                                m_hasPlayedAudio = true;
                            }
                            Destroy(m_impacts[i].transform.GetComponentInParent<DemonBase>().gameObject, 1f);
                        }
                    }
                    else
                    {
                        m_fireCovered = true;
                        m_currentFireAltitude = Mathf.Clamp01(m_impacts[i].distance / m_height);
                        m_propertyBlock.SetFloat("_height", Mathf.Clamp01(m_currentFireAltitude));
                        m_spriteRenderer.SetPropertyBlock(m_propertyBlock);
                    }
                }
            }
        }
        else
        {
            m_fireCovered = false;
            m_currentFireAltitude = Mathf.Clamp01(Mathf.Lerp(m_currentFireAltitude, 1, Time.deltaTime));
            m_propertyBlock.SetFloat("_height", Mathf.Clamp01(m_currentFireAltitude));
            m_spriteRenderer.SetPropertyBlock(m_propertyBlock);
        }
    }

    IEnumerator ResetPlayedAudio(float time)
    {
        yield return new WaitForSeconds(time);
        m_hasPlayedAudio = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (LayerMask.LayerToName(collision.transform.gameObject.layer) == "Body" || LayerMask.LayerToName(collision.transform.gameObject.layer) == "Player" || collision.GetComponent<DestructiblePlatform>() != null)
            m_detections.Add(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Debug.LogError(collision.transform);
        if (m_detections.Contains(collision.gameObject))
            m_detections.Remove(collision.gameObject);
    }
}
