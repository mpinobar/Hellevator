using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalTransportData
{
    private Rigidbody2D m_demonRgb;
    private bool m_isVelocityApplied;

    public Rigidbody2D DemonRgb { get => m_demonRgb; set => m_demonRgb = value; }
    public bool IsVelocityApplied { get => m_isVelocityApplied; set => m_isVelocityApplied = value; }

    public HorizontalTransportData(Rigidbody2D demon, bool isApplied)
    {
        m_demonRgb = demon;
        m_isVelocityApplied = isApplied;
    }
}
