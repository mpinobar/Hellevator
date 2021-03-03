using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class ActivatedBase : MonoBehaviour
{
    private Puzzle m_belongingPuzzle;
    private bool active;
	[SerializeField] bool m_deactivatesOnPuzzleCompletion;
    public bool Active { get => active; set => active = value; }
    public Puzzle BelongingPuzzle { get => m_belongingPuzzle; set => m_belongingPuzzle = value; }

    public Action OnActivated;
    public virtual void Activate()
    {
        active = true;
        OnActivated?.Invoke();
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
        OnActivated?.Invoke();
        if (m_deactivatesOnPuzzleCompletion)
        {
            gameObject.SetActive(false);
        }
    }
}
