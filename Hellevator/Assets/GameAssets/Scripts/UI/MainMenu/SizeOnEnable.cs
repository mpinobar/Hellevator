using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SizeOnEnable : MonoBehaviour
{
    [SerializeField] float timeToExpand = 0.5f;
    float expandTimer = 0;
    float extensionPercentage = 0;

    private void OnEnable()
    {
        expandTimer = 0;
        extensionPercentage = 0;
        StartCoroutine(Expand());
    }

    // Update is called once per frame
    IEnumerator Expand()
    {
        while (expandTimer < timeToExpand)
        {
            expandTimer += Time.unscaledDeltaTime;
            extensionPercentage = Mathf.Clamp01(expandTimer / timeToExpand);
            transform.localScale = new Vector3(extensionPercentage, 1, 1);
            yield return new WaitForSecondsRealtime(Time.unscaledDeltaTime);
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
