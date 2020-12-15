using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableKey : MonoBehaviour
{
    [SerializeField] Key key;
    [SerializeField] bool m_checkPlayerPrefs;
	[SerializeField] AudioClip m_pickUpClip = null;

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
			MusicManager.Instance.PlayAudioSFX(m_pickUpClip, false, 2f);
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
                LevelManager.Instance.HasKitchenKey = true;
                Destroy(gameObject);
                PlayerPrefs.SetInt(key.ToString(), 1);
                PlayerPrefs.Save();
            }
        }
    }
}
