using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : TemporalSingleton<MusicManager>
{
    [Range (0,1)]
    [SerializeField] static float m_sfxVolume = 0.25f;
    [Range(0, 1)]
    [SerializeField] static float m_musicVolume = 0.5f;
    List<AudioSource> m_sourcesList;

    [SerializeField] List<AudioClip> m_backgroundMusicClips;
    AudioSource m_BGM;
    float m_spatialBlendSFX = 0.35f;
    int m_musicClipIndex;
        
    public static float MusicVolume
    {
        get => m_musicVolume;
        set
        {

            m_musicVolume = value;

        }
    }
    public static float SfxVolume { get => m_sfxVolume; set => m_sfxVolume = value; }

    public void ChangeBGMVolume(float v)
    {
        m_BGM.volume = v;
    }

    public override void Awake()
    {
        base.Awake();
        m_sourcesList = new List<AudioSource>();
        m_BGM = gameObject.AddComponent<AudioSource>();
        m_BGM.volume = m_musicVolume;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.G))
        {
            QueueOutOfTuneClip();
        }
        if (!m_BGM.isPlaying && m_backgroundMusicClips != null)
        {
            //int aux = m_currentBGMClip;
            //while (aux == m_currentBGMClip && m_backgroundMusicClips.Count > 1)
            //{
            //    m_currentBGMClip = Random.Range(0, m_backgroundMusicClips.Count);
            //}
            
            m_BGM.clip = m_backgroundMusicClips[m_musicClipIndex];
            m_musicClipIndex = (m_musicClipIndex + 1) % 2;
            m_BGM.Play();
        }


        int d = 0;
        for (int i = 0; i < m_sourcesList.Count; i++)
        {
            if (!m_sourcesList[i].isPlaying)
            {
                d++;
            }
        }
    }
    public void PlayAudioMusic(AudioClip clip, bool looping)
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
                m_sourcesList[i].loop = looping;
                m_sourcesList[i].volume = m_musicVolume;
                m_sourcesList[i].Play();
                foundFreeSource = true;
                return;
            }
        }
        if (!foundFreeSource)
        {
            m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
            m_sourcesList[m_sourcesList.Count - 1].playOnAwake = false;
            m_sourcesList[m_sourcesList.Count - 1].clip = clip;
            m_sourcesList[m_sourcesList.Count - 1].loop = looping;
            m_sourcesList[m_sourcesList.Count - 1].volume = m_musicVolume;
            m_sourcesList[m_sourcesList.Count - 1].Play();
        }
    }

    public AudioSource PlayAudioSFX(AudioClip clip, bool looping)
    {
        if (m_sourcesList == null)
        {
            m_sourcesList = new List<AudioSource>();
        }

        if (m_sourcesList.Count == 0)
        {
            CreateAudioSourceAndSetParametersForSFX(clip, looping);
            return m_sourcesList[m_sourcesList.Count - 1];
        }
        else
        {

            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (!m_sourcesList[i].isPlaying)
                {
                    m_sourcesList[i].clip = clip;
                    m_sourcesList[i].volume = m_sfxVolume;
                    m_sourcesList[i].spatialBlend = m_spatialBlendSFX;
                    m_sourcesList[i].loop = looping;
                    m_sourcesList[i].Play();

                    return m_sourcesList[i];
                }
            }
            CreateAudioSourceAndSetParametersForSFX(clip, looping);
            
            return m_sourcesList[m_sourcesList.Count - 1];
        }
    }

    

    public AudioSource PlayAudioSFX(AudioClip clip, bool looping, float volumeModifier)
    {
        if (m_sourcesList == null)
        {
            m_sourcesList = new List<AudioSource>();
        }

        if (m_sourcesList.Count == 0)
        {
            CreateAudioSourceAndSetParametersForSFX(clip, looping);
            return m_sourcesList[m_sourcesList.Count - 1];
        }
        else
        {

            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (!m_sourcesList[i].isPlaying)
                {
                    m_sourcesList[i].clip = clip;
                    m_sourcesList[i].volume = m_sfxVolume * volumeModifier;
                    m_sourcesList[i].loop = looping;
                    m_sourcesList[i].spatialBlend = m_spatialBlendSFX;
                    m_sourcesList[i].Play();

                    return m_sourcesList[i];
                }
            }

            CreateAudioSourceAndSetParametersForSFX(clip, looping);
            return m_sourcesList[m_sourcesList.Count - 1];
        }
    }

    private void CreateAudioSourceAndSetParametersForSFX(AudioClip clip, bool looping)
    {
        m_sourcesList.Add(gameObject.AddComponent<AudioSource>());
        m_sourcesList[m_sourcesList.Count - 1].playOnAwake = false;
        m_sourcesList[m_sourcesList.Count - 1].spatialBlend = m_spatialBlendSFX;
        m_sourcesList[m_sourcesList.Count - 1].clip = clip;
        m_sourcesList[m_sourcesList.Count - 1].volume = m_sfxVolume;
        m_sourcesList[m_sourcesList.Count - 1].loop = looping;
        m_sourcesList[m_sourcesList.Count - 1].Play();
    }

    public void QueueOutOfTuneClip()
    {
        m_musicClipIndex = 2;
    }

}
