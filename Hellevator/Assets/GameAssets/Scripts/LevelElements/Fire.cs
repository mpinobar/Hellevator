using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    Vector2 m_startingPoint;
    Vector2 m_endPoint;
    float m_height;
    SpriteRenderer m_spriteRenderer;
    MaterialPropertyBlock m_propertyBlock;
    float m_currentFireAltitude = 1;
    // Start is called before the first frame update
    void Start()
    {
        m_height = transform.localScale.x;
        m_startingPoint = transform.position - Vector3.up * m_height * 0.5f;
        m_endPoint = transform.position + Vector3.up * m_height * 0.5f;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_propertyBlock = new MaterialPropertyBlock();
        m_spriteRenderer.GetPropertyBlock(m_propertyBlock);
    }
    //RaycastHit2D raycastHit;
    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(m_startingPoint, (m_endPoint - m_startingPoint) * m_currentFireAltitude, Color.green);
        RaycastHit2D raycastHit = Physics2D.Raycast(m_startingPoint, m_endPoint - m_startingPoint, m_height * m_currentFireAltitude);
        if (raycastHit.transform != null)
        {
            //Debug.LogError(raycastHit.transform.name);
            //m_spriteRenderer.GetPropertyBlock(m_propertyBlock);
            float distance = Vector2.Distance(raycastHit.transform.position, m_startingPoint);
            m_currentFireAltitude = distance / m_height;
            m_propertyBlock.SetFloat("_height", Mathf.Clamp01(m_currentFireAltitude));
            m_spriteRenderer.SetPropertyBlock(m_propertyBlock);
        }
        else
        {
            m_currentFireAltitude = Mathf.Lerp(m_currentFireAltitude, 1, Time.deltaTime);
            m_propertyBlock.SetFloat("_height", Mathf.Clamp01(m_currentFireAltitude));
            m_spriteRenderer.SetPropertyBlock(m_propertyBlock);
        }
    }
}
