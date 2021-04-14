using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CauldronExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Collider2D[]cols = Physics2D.OverlapCircleAll(transform.position,30);
        for (int i = 0; i < cols.Length; i++)
        {
            if(cols[i].TryGetComponent(out DestructiblePlatform platform))
            {
                //Debug.LogError(platform.name);
                platform.StartDestroyPlatform();
                platform.GetComponent<Collider2D>().enabled = false;
            }
            else if(cols[i].TryGetComponent(out Boss boss))
            {
                boss.DamageBoss();
            }
        }
    }


}
