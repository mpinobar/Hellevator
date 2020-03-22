using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesWeightData 
{
    private DemonBase m_associatedDemon;
    private List<Collider2D> colliders;

    public DemonBase AssociatedDemon { get => m_associatedDemon; set => m_associatedDemon = value; }
    public List<Collider2D> Colliders { get => colliders; set => colliders = value; }

    public SpikesWeightData(DemonBase demon,Collider2D c)
    {
        colliders = new List<Collider2D>();
        colliders.Add(c);
        m_associatedDemon = demon;
    }
}
