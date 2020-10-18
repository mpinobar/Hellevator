using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableKey : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] bool m_checkPlayerPrefs;

    private void Start()
    {
        if (m_checkPlayerPrefs)
        {
            if (PlayerPrefs.GetInt(key.ToString()) == 1)
            {
                Destroy(gameObject);
            }
        }

    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponentInParent<DemonBase>())
        {
            if (m_checkPlayerPrefs)
            {

                if (PlayerPrefs.GetInt(key.ToString()) == 0)
                {
                    Destroy(gameObject);
                    PlayerPrefs.SetInt(key.ToString(), 1);
                    PlayerPrefs.Save();
                }
            }
            else
            {
                Destroy(gameObject);
                PlayerPrefs.SetInt(key.ToString(), 1);
                PlayerPrefs.Save();
            }
        }
    }
}
