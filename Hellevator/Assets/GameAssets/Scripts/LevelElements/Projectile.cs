using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 2f;

    [SerializeField] float timeToDeactivate = 5f;
    float timeToDeactivateTimer;

    private void OnEnable()
    {
        Vector3 pos = transform.position;
        pos.z = 0;
        transform.position = pos;
        timeToDeactivateTimer = timeToDeactivate;
    }
    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            GetComponent<Rigidbody2D>().velocity = transform.up * Speed;            
        }
    }

    private void Update()
    {
        timeToDeactivateTimer -= Time.deltaTime;
        if(timeToDeactivateTimer <= 0)
        {
            DeactivateProjectile();
        }
    }

    public void DeactivateProjectile()
    {
        gameObject.SetActive(false);
        transform.localPosition = Vector3.zero;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {        
        if (collision.GetComponentInParent<DemonBase>() != null)
        {
            collision.GetComponentInParent<DemonBase>().Die(true);
        }
        gameObject.SetActive(false);
    }
}
