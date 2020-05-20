using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager>
{
    [SerializeField] float m_sfxVolume;
    [SerializeField] float m_musicVolume;
    List<AudioSource> m_sourcesList;

    public override void Awake()
    {
        base.Awake();
        m_sourcesList = new List<AudioSource>();
    }


    public void PlayAudioMusic(AudioClip clip)
    {
        if (m_sourcesList.Count == 0)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
        }

        bool foundFreeSource = false;

        for (int i = 0; i < m_sourcesList.Count; i++)
        {
            if (!m_sourcesList[i].isPlaying)
            {
                m_sourcesList[i].clip = clip;
                m_sourcesList[i].volume = m_musicVolume;
                m_sourcesList[i].Play();
                foundFreeSource = true;
                return;
            }
        }
        if (!foundFreeSource)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
            m_sourcesList[m_sourcesList.Count].clip = clip;
            m_sourcesList[m_sourcesList.Count].volume = m_musicVolume;
            m_sourcesList[m_sourcesList.Count].Play();
        }
    }

    public void PlayAudioSFX(AudioClip clip)
    {
        if(m_sourcesList.Count == 0)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
        }

        bool foundFreeSource = false;

        for (int i = 0; i < m_sourcesList.Count; i++)
        {
            if (!m_sourcesList[i].isPlaying)
            {
                m_sourcesList[i].clip = clip;
                m_sourcesList[i].volume = m_sfxVolume;
                m_sourcesList[i].Play();
                foundFreeSource = true;
                return;
            }
        }
        if (!foundFreeSource)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
            m_sourcesList[m_sourcesList.Count].clip = clip;
            m_sourcesList[m_sourcesList.Count].volume = m_sfxVolume;
            m_sourcesList[m_sourcesList.Count].Play();
        }
    }
}
