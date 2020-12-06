using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BugFixK1 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(KillAllCharactersAbove), 0.1f);
    }

    private void KillAllCharactersAbove()
    {
		Debug.LogError("Killin all characters that are children of " + name);
		for (int i = 0; i < transform.childCount; i++)
		{
			if (transform.GetChild(i).GetComponent<DemonBase>() != null)
			{
				Debug.LogError("Found " + transform.GetChild(i).gameObject.name);
				Destroy(transform.GetChild(i).gameObject);
				i--;
			}
		}
	}
}
