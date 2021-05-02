using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableHorizontalNavigator : Selectable
{
    [SerializeField] bool isRight;
    
    [SerializeField] UICollectibles collec;

    public override void NavigateLeft()
    {
        if (isRight)
        {
            base.NavigateLeft();
        }
        else
            collec.Previous();
    }

    public override void NavigateRight()
    {
        if (isRight)
            collec.Next();
        else
        {
            base.NavigateRight();
        }
    }

}
