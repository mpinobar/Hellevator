using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] float explosionRadius;
    [SerializeField] float explosionForce;
    [SerializeField] LayerMask explosionInteractionLayerMask;
    [SerializeField] List<GameObject> limbsToUnparent;

    public void CreateExplosion()
    {
        GetComponent<DemonBase>().RagdollLogicCollider.gameObject.SetActive(false);

        ExplosionVisuals();
        Collider2D [] colliders = Physics2D.OverlapCircleAll(transform.position,explosionRadius,explosionInteractionLayerMask);
        DemonBase demonInRange;
        DestructibleWall explodingWall;
        for (int i = 0; i < colliders.Length; i++)
        {
            demonInRange = colliders[i].GetComponentInParent<DemonBase>();
            explodingWall = colliders[i].GetComponentInParent<DestructibleWall>();
            if (demonInRange && demonInRange != GetComponentInParent<DemonBase>())
            {
                demonInRange.Die(true);
            }
            if (explodingWall)
            {
                explodingWall.Explode(transform.position, explosionForce);
            }
        }
        PossessionManager.Instance.RemoveDemonPossession(transform);

        UnparentLimbs();
        GetComponent<DemonBase>().enabled = false;
    }

    public void ExplosionVisuals()
    {
        explosionParticles.Play();
        for (int i = 0; i < explosionParticles.transform.childCount; i++)
        {
            explosionParticles.transform.GetChild(i).GetComponent<ParticleSystem>().Play();
        }
    }
    public void UnparentLimbs()
    {
        for (int i = 0; i < limbsToUnparent.Count; i++)
        {
            limbsToUnparent[i].transform.parent = null;
            limbsToUnparent[i].GetComponent<HingeJoint2D>().enabled = false;
            limbsToUnparent[i].GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            limbsToUnparent[i].GetComponent<Rigidbody2D>().AddForce((Vector2.up + Random.Range(-2, 2) * Vector2.right) * explosionForce, ForceMode2D.Impulse);
        }
    }
}
