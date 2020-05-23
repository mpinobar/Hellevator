using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Collider2D))]
public class CheckPoint : MonoBehaviour
{
    bool m_opening;
    [SerializeField] private DemonBase m_demonToSpawn;
    private float m_openingValue;
    SpriteRenderer m_spr;
    

    private void Awake()
    {
        m_spr = GetComponent<SpriteRenderer>();
        m_openingValue = 1;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponentInParent<DemonBase>() == PosesionManager.Instance.ControlledDemon)
        {
            ActivateCheckPoint();
            m_opening = true;
        }
    }


    private void Update()
    {
        if (m_opening)
        {
            m_openingValue -= Time.deltaTime;
            m_spr.material.SetFloat("_Opening", m_openingValue);
            if(m_openingValue <= 0)
            {
                m_opening = false;
            }
        }
    }

    private void ActivateCheckPoint()
    {
        LevelManager.Instance.SetLastCheckPoint(this);
        GetComponent<SpriteRenderer>().material.SetFloat("_Active", 1);
        GetComponent<SpriteRenderer>().material.SetFloat("_Opening", 0);
    }

    /// <summary>
    /// Spawns player at checkpoint location, summoning and possessing the instantiated type of demon
    /// </summary>
    public void SpawnPlayer()
    {
        DemonBase spawnedDemon = Instantiate(m_demonToSpawn, transform.position - Vector3.up*2, Quaternion.identity);
        spawnedDemon.enabled = true;
        spawnedDemon.SetControlledByPlayer();
        ActivateCheckPoint();
        //CameraManager.Instance.ChangeCamTarget();
        InputManager.Instance.UpdateDemonReference();
    }
}
