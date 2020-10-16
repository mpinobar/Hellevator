using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    float speed = 2f;

    [SerializeField] float timeToDeactivate = 5f;

    public float Speed
    {
        get => speed;
        set
        {
            speed = value;
            GetComponent<Rigidbody2D>().velocity = transform.up * Speed;
            Invoke(nameof(DeactivateProjectile), timeToDeactivate);
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
