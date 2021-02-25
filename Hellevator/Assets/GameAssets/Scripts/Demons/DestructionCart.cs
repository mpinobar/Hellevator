using UnityEngine;

public class DestructionCart : MonoBehaviour
{
    [SerializeField] float m_explosionForceOnImpact = 50f;
    public bool m_active;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_active)
        {
            Spikes spikes = collision.GetComponentInParent<Spikes>();
            if (spikes)
            {
                Rigidbody2D spikeRB = spikes.GetComponent<Rigidbody2D>();
                spikeRB.isKinematic = false;
                spikeRB.AddForce((-transform.right + transform.up).normalized * m_explosionForceOnImpact, ForceMode2D.Impulse);
                spikeRB.GetComponent<Collider2D>().enabled = false;
            }

            DestructibleWall wall = collision.GetComponentInParent<DestructibleWall>();
            if (wall)
            {
                wall.Explode(transform.position, m_explosionForceOnImpact);
            }


            BasicZombie character = collision.GetComponentInParent<BasicZombie>();
            if (character)
            {
                if (character.IsControlledByPlayer)
                {
                    character.Die(true);
                }
                character.UnparentBodyParts(0f);
            }
        }
        
    }
}
