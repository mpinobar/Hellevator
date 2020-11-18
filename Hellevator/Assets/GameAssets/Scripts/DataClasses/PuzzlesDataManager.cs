using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzlesDataManager
{
    public static bool CheckPuzzle(string puzzleID)
    {
        if (PlayerPrefs.HasKey(puzzleID))
        {
            if(PlayerPrefs.GetInt(puzzleID) == 1)
            {
                return true;
            }
            return false;
        }
        return false;
    }

    public static void SetPuzzleSolved(string puzzleID)
    {
        PlayerPrefs.SetInt(puzzleID, 1);
    }
}
