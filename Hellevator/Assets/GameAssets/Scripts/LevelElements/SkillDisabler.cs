using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDisabler : MonoBehaviour
{    
    public void DisableSkills(Collider2D collision)
    {
        collision.TryGetComponent(out Explosion explosive);
        if (explosive)
        {
            //Destroy(explosive);
            explosive.SetCantExplode();
        }

        collision.TryGetComponent(out Petrification petrification);
        if (petrification)
        {
            petrification.SetCantPetrify();
        }
    }
}
