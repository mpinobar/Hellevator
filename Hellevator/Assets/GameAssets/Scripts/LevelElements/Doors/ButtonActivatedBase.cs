using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ButtonActivatedBase : MonoBehaviour
{
	public abstract void Activate();

	public virtual void Deactivate()
    {

    }
}
