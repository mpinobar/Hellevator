using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : PersistentSingleton<AudioManager>
{
    [Range (0,1)]
    [SerializeField] static float m_sfxVolume = 0.25f;
    [Range(0, 1)]
    [SerializeField] static float m_musicVolume = 0.5f;
    List<AudioSource> m_sourcesList;
    [SerializeField] float m_volumeFadeTransitionDuration = 1f;
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
        m_BGM.volume = v * 0.5f;
    }

    public override void Awake()
    {
        base.Awake();
        m_sourcesList = new List<AudioSource>();
        m_BGM = gameObject.AddComponent<AudioSource>();
        m_BGM.volume = m_musicVolume * 0.5f;
        m_BGM.loop = true;
        if (m_backgroundMusicClips != null)
        {
            m_BGM.clip = m_backgroundMusicClips[m_musicClipIndex];
            m_BGM.Play();
        }
    }

    internal void CheckMusic()
    {
        if (!m_BGM.isPlaying)
        {
            StartGameplayMusic();
        }
    }

    public void StopMusic()
    {
        if (m_BGM)
            m_BGM.Stop();
    }

    public void StartGameplayMusic()
    {
        if (m_backgroundMusicClips != null && m_backgroundMusicClips.Count > 0)
        {
            m_BGM.Stop();
            m_BGM.clip = m_backgroundMusicClips[m_musicClipIndex];
            m_BGM.loop = true;
            m_BGM.Play();
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
                m_sourcesList[i].volume = m_musicVolume * 0.5f;
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
            m_sourcesList[m_sourcesList.Count - 1].volume = m_musicVolume * 0.5f;
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

            int amountPlayingClip = 1; //empieza a 1 porque se cuenta el que queremos añadir
            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (m_sourcesList[i].isPlaying && m_sourcesList[i].clip == clip)
                {
                    amountPlayingClip++;
                }
            }

            if(amountPlayingClip > 1)
            {
                for (int i = 0; i < m_sourcesList.Count; i++)
                {
                    if (m_sourcesList[i].isPlaying && m_sourcesList[i].clip == clip)
                    {
                        m_sourcesList[i].volume = SfxVolume / amountPlayingClip;
                    }
                }
            }
            

            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (m_sourcesList[i].isPlaying && m_sourcesList[i].clip == clip)
                    return null;

                if (!m_sourcesList[i].isPlaying)
                {
                    m_sourcesList[i].clip = clip;
                    m_sourcesList[i].volume = m_sfxVolume / amountPlayingClip;
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
        float volume = volumeModifier;
        if (m_sourcesList.Count == 0)
        {
            CreateAudioSourceAndSetParametersForSFX(clip, looping);
            return m_sourcesList[m_sourcesList.Count - 1];
        }
        else
        {
            int amountPlayingClip = 1; //empieza a 1 porque se cuenta el que queremos añadir
            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (m_sourcesList[i].isPlaying && m_sourcesList[i].clip == clip)
                {
                    amountPlayingClip++;
                }
            }

            for (int i = 0; i < m_sourcesList.Count; i++)
            {
                if (m_sourcesList[i].isPlaying && m_sourcesList[i].clip == clip)
                {
                    m_sourcesList[i].volume = SfxVolume * volumeModifier / amountPlayingClip;
                }
            }


            for (int i = 0; i < m_sourcesList.Count; i++)
            {

                if (!m_sourcesList[i].isPlaying)
                {
                    m_sourcesList[i].clip = clip;
                    m_sourcesList[i].volume = SfxVolume * volumeModifier / amountPlayingClip;
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

    public void SetBackgroundMusicToKitchen()
    {
        if (m_musicClipIndex != 2)
        {
            m_musicClipIndex = 2;
            StartCoroutine(MusicVolumeFadeInAndOut(m_volumeFadeTransitionDuration));

        }
    }

    IEnumerator MusicVolumeFadeInAndOut(float transitionTime)
    {
        float halfTime = transitionTime * 0.5f;
        float startingVolume = m_BGM.volume;
        float currentTime = 0;
        float volumeChangeDelta = startingVolume*Time.deltaTime/halfTime;

        while (currentTime < halfTime)
        {
            currentTime += Time.deltaTime;
            m_BGM.volume = Mathf.Clamp(m_BGM.volume - volumeChangeDelta, 0, startingVolume);
            yield return null;
        }
        StartGameplayMusic();
        while (currentTime < transitionTime)
        {
            currentTime += Time.deltaTime;
            m_BGM.volume = Mathf.Clamp(m_BGM.volume + volumeChangeDelta, 0, startingVolume);
            yield return null;
        }
        m_BGM.volume = startingVolume;
    }
    public void SetBackgroundMusicToRestaurant()
    {
        if (m_musicClipIndex != 1)
        {
            m_musicClipIndex = 1;
            StartCoroutine(MusicVolumeFadeInAndOut(m_volumeFadeTransitionDuration));

        }
    }
    public void SetBackgroundMusicToMenu()
    {
        if (m_musicClipIndex != 0)
        {
            m_musicClipIndex = 0;
            StartCoroutine(MusicVolumeFadeInAndOut(m_volumeFadeTransitionDuration));

        }
    }

    /// <summary>
    /// Loops all sfx audio sources and stops if it finds a source playing the clip
    /// </summary>
    /// <param name="clipToStop">The clip to stop playing</param>
    public void StopSFX(AudioClip clipToStop)
    {
        for (int i = 0; i < m_sourcesList.Count; i++)
        {
            if(m_sourcesList[i].clip == clipToStop && m_sourcesList[i].isPlaying)
            {
                m_sourcesList[i].Stop();
            }
        }
    }

}
