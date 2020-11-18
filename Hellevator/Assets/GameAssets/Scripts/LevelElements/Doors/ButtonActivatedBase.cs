using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonActivatedBase : MonoBehaviour
{
    private Puzzle m_belongingPuzzle;
    private bool active;
    public bool Active { get => active; set => active = value; }
    public Puzzle BelongingPuzzle { get => m_belongingPuzzle; set => m_belongingPuzzle = value; }

    public virtual void Activate()
    {
        active = true;
        if (m_belongingPuzzle)
        {
            m_belongingPuzzle.TrySetPuzzleSolved();
        }
    }

	public virtual void Deactivate()
    {
        active = false;
    }

    public virtual void ActivateImmediately()
    {
        active = true;
    }
}
