using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagdollTransform 
{
    private int m_id;
    private Vector3 m_position;
    private Quaternion m_rotation;

    public RagdollTransform(int id, Vector3 position, Quaternion rotation)
    {
        this.m_id = id;
        this.m_position = position;
        this.m_rotation = rotation;
    }

    public Vector3 Position { get => m_position; set => m_position = value; }
    public Quaternion Rotation { get => m_rotation; set => m_rotation = value; }
    public int Id { get => m_id; set => m_id = value; }
}
