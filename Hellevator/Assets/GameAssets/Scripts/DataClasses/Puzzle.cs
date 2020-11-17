using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour
{
    [SerializeField] string m_ID;

    [SerializeField] ButtonActivatedBase [] m_completeWhenActive;

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
    }

    public void TrySetPuzzleSolved()
    {
        Debug.LogError("Part of a puzzle was solved. Checking if all are");
        for (int i = 0; i < m_completeWhenActive.Length; i++)
        {
            if (!m_completeWhenActive[i].Active)
            {
                Debug.LogError("Puzzle with id " + m_ID + " was NOT solved");
                return;
            }
        }

        Debug.LogError("Puzzle with id " + m_ID + " was solved. Saving puzzle completion");
        PuzzlesDataManager.SetPuzzleSolved(m_ID);

    }

    public void CheckSolvePuzzle()
    {
        Debug.LogError("Checking if puzzle with id " + m_ID + " was solved");
        if (PuzzlesDataManager.CheckPuzzle(m_ID))
        {
            Debug.LogError("Puzzle was solved. Unlocking all puzzle parts");
            for (int i = 0; i < m_completeWhenActive.Length; i++)
            {
                m_completeWhenActive[i].ActivateImmediately();
            }
        }
        else
        {
            Debug.LogError("Puzzle with id " + m_ID + " was NOT solved");
        }
    }

}
