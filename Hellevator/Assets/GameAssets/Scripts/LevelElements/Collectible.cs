using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    [SerializeField]
    string m_ID;

    void Start()
    {
        if (PlayerPrefs.HasKey(m_ID) && PlayerPrefs.GetInt(m_ID) > 0)
        {
            Destroy(gameObject);
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        DemonBase character = collision.GetComponentInParent<DemonBase>();
        if (character && character.IsControlledByPlayer)
        {
            Collect();
        }
    }
    public void Collect()
    {
        if (clip)
            AudioManager.Instance.PlayAudioSFX(clip, false);
        AchievementsManager.AddCollectible();
        PlayerPrefs.SetInt(m_ID, 1);
        Destroy(gameObject);
    }
}
