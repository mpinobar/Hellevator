using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Petrification : MonoBehaviour
{
    [SerializeField] DestructiblePlatform prefabToConvertInto;
    [SerializeField] bool usesGravity;

    /// <summary>
    /// Instantiates a platform, possesses a new demon and
    /// </summary>
    public void Petrify()
    {
        Rigidbody2D platform = Instantiate(prefabToConvertInto,transform.position,Quaternion.identity).GetComponent<Rigidbody2D>();

        if (!usesGravity)
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
