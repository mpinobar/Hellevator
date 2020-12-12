using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatFlyAway : MonoBehaviour
{
    [SerializeField] float m_flyAwayTime = 2f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.LogError("Player scared away bat: " + name);
        StartCoroutine(FlyAway());
    }

    IEnumerator FlyAway()
    {
        float scale = transform.localScale.x;
        float decreaseRate = 1/m_flyAwayTime;
        while (scale >= 0.1f)
        {
            scale -= Time.deltaTime * decreaseRate;
            transform.localScale = Vector3.one * scale;
            yield return null;
        }
        gameObject.SetActive(false);
    }
}
