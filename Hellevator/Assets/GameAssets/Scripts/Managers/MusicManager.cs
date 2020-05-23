using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : PersistentSingleton<MusicManager>
{
    [Range (0,1)]
    [SerializeField] float m_sfxVolume = 1;
    [Range(0, 1)]
    [SerializeField] float m_musicVolume = 1;
    List<AudioSource> m_sourcesList;

    [SerializeField] List<AudioClip> m_backgroundMusicClips;
    AudioSource m_BGM;
    int m_currentBGMClip;

    public float MusicVolume { get => m_musicVolume; set => m_musicVolume = value; }
    public float SfxVolume { get => m_sfxVolume; set => m_sfxVolume = value; }

    public override void Awake()
    {
        base.Awake();
        m_sourcesList = new List<AudioSource>();
        m_BGM = gameObject.AddComponent<AudioSource>();
        m_BGM.volume = m_musicVolume;
    }

    private void Update()
    {
        if (!m_BGM.isPlaying)
        {
            int aux = m_currentBGMClip;
            while (aux == m_currentBGMClip)
            {
                m_currentBGMClip = Random.Range(0, m_backgroundMusicClips.Count);
            }
            m_BGM.clip = m_backgroundMusicClips[m_currentBGMClip];
            m_BGM.Play();
        }
    }
    public void PlayAudioMusic(AudioClip clip)
    {
        if (m_sourcesList == null)
        {
            m_sourcesList = new List<AudioSource>();
        }

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

    public AudioSource PlayAudioSFX(AudioClip clip)
    {
        if(m_sourcesList == null)
        {
            m_sourcesList = new List<AudioSource>();
        }

        if(m_sourcesList.Count == 0)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
            m_sourcesList[m_sourcesList.Count - 1].clip = clip;
            m_sourcesList[m_sourcesList.Count - 1].volume = m_sfxVolume;
            m_sourcesList[m_sourcesList.Count - 1].Play();
            return m_sourcesList[m_sourcesList.Count - 1];
        }
        else
        {
            bool foundFreeSource = false;

            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (!m_sourcesList[i].isPlaying)
                {
                    m_sourcesList[i].clip = clip;
                    m_sourcesList[i].volume = m_sfxVolume;
                    m_sourcesList[i].Play();
                    foundFreeSource = true;
                    return m_sourcesList[i];
                }                
            }
            if (!foundFreeSource)
            {
                m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
                m_sourcesList[m_sourcesList.Count - 1].clip = clip;
                m_sourcesList[m_sourcesList.Count - 1].volume = m_sfxVolume;
                m_sourcesList[m_sourcesList.Count - 1].Play();
                return m_sourcesList[m_sourcesList.Count - 1];
            }
            else
            {
                return null;
            }
        }        
    }
}
