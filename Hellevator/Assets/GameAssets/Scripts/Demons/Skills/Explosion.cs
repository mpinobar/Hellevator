using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionForce;
    [SerializeField] LayerMask explosionInteractionLayerMask;

    public void CreateExplosion()
    {
        ExplosionVisuals();
        Collider2D [] colliders = Physics2D.OverlapCircleAll(transform.position,explosionRadius,explosionInteractionLayerMask);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponentInParent<DemonBase>())
            {
                colliders[i].GetComponentInParent<DemonBase>().Die(true);
            }
        }

    }

    public void ExplosionVisuals()
    {

    }

}
