using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] string m_ID;

    [SerializeField] ActivatedBase [] m_completeWhenActive;
    [SerializeField] ActivatedBase [] m_secondaryActivations;

    private void OnEnable()
    {
        CheckSolvePuzzle();

        SetPuzzleReferences();
    }

    private void SetPuzzleReferences()
    {
        for (int i = 0; i < m_completeWhenActive.Length; i++)
        {
            m_completeWhenActive[i].BelongingPuzzle = this;
        }
        if (m_secondaryActivations != null && m_secondaryActivations.Length > 0)
            for (int i = 0; i < m_secondaryActivations.Length; i++)
            {
                m_secondaryActivations[i].BelongingPuzzle = this;
            }
    }

    public void TrySetPuzzleSolved()
    {
        for (int i = 0; i < m_completeWhenActive.Length; i++)
        {
            if (!m_completeWhenActive[i].Active)
            {
                return;
            }
        }
        PuzzlesDataManager.SetPuzzleSolved(m_ID);

    }

    public void CheckSolvePuzzle()
    {
        if (PuzzlesDataManager.CheckPuzzle(m_ID))
        {
            for (int i = 0; i < m_completeWhenActive.Length; i++)
            {
                m_completeWhenActive[i].ActivateImmediately();
            }
            if (m_secondaryActivations != null && m_secondaryActivations.Length > 0)
            {
                for (int i = 0; i < m_secondaryActivations.Length; i++)
                {
                    m_secondaryActivations[i].ActivateImmediately();
                }
            }
        }
    }

}
