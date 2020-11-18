using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Halo : MonoBehaviour
{

    [SerializeField] float m_timeToDisappear = 0.25f;
    float m_timeToDisappearTimer;
    Transform m_target;
    float m_offsetY;
    [SerializeField] float m_xScaleReductionSpeed;

    public Transform Target { get => m_target; set => m_target = value; }

    // Start is called before the first frame update
    void Start()
    {
        m_timeToDisappearTimer = m_timeToDisappear;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target)
        {
            m_timeToDisappearTimer -= Time.deltaTime;
            transform.localScale -= Vector3.right * Time.deltaTime * m_xScaleReductionSpeed;
            transform.position = Target.position + m_offsetY * Vector3.up;
            if (m_timeToDisappearTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            SetTarget(PossessionManager.Instance.ControlledDemon.transform,30f);
            //Destroy(gameObject);
        }
    }

    public void SetTarget(Transform targetToFollow,float offsetY)
    {
        m_target = targetToFollow;
        m_offsetY = offsetY;
    }
}
