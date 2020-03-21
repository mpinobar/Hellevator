using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTransform 
{
    private int id;
    private Vector3 position;
    private Quaternion rotation;

    public RagdollTransform(int id, Vector3 position, Quaternion rotation)
    {
        this.id = id;
        this.position = position;
        this.rotation = rotation;
    }

    public Vector3 Position { get => position; set => position = value; }
    public Quaternion Rotation { get => rotation; set => rotation = value; }
    public int Id { get => id; set => id = value; }
}
