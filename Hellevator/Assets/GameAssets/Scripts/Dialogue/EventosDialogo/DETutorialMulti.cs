using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DETutorialMulti : DialogEvent
{
    public override void ActivateEvent()
    {
        if (PlayerPrefs.GetInt("MultiIsUnlocked") == 1)
        {
            Destroy(this.gameObject);
        }
        else
        {
            PlayerPrefs.SetInt("MultiIsUnlocked", 1);
            PossessionManager.Instance.MultiplePossessionIsUnlocked = true;
        }
    }
}