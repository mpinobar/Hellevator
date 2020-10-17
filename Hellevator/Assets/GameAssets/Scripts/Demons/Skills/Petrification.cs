using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petrification : MonoBehaviour
{
    [SerializeField] DestructiblePlatform m_prefabToConvertInto;
    [SerializeField] bool m_usesGravity;

    /// <summary>
    /// Instantiates a platform, possesses a new demon and
    /// </summary>
    public void Petrify()
    {
        Rigidbody2D platform = Instantiate(m_prefabToConvertInto,transform.position,Quaternion.identity).GetComponent<Rigidbody2D>();

        if (!m_usesGravity)
        {
            platform.isKinematic = true;
        }
        else
        {
            platform.velocity = transform.root.GetComponent<Rigidbody2D>().velocity;
            platform.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        
        platform.GetComponent<DestructiblePlatform>().WillReappear = false;

        PossessionManager.Instance.RemoveDemonPossession(transform);
        gameObject.SetActive(false);
    }
}
