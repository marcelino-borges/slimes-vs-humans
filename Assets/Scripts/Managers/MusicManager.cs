using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [HideInInspector]
    public static MusicManager instance;
    [HideInInspector]
    public AudioSource audioSource;
    [HideInInspector]
    public AudioClip nextMusic;
    //public AudioClip nextMusic2;
    [HideInInspector]
    public Animator animator;
    public float musicTransitionDuration = 2f;

    public float currentVolume = .15f;
    public float CurrentVolume { get => isMuted ? 0 : currentVolume; set => currentVolume = value; }

    public float maxVolume = .15f;
    public bool isMuted = false;
    public bool fadingIn, fadingOut;
    private bool changeMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Start()
    {
        audioSource.volume = CurrentVolume;
    }

    private void Update()
    {
        if (fadingIn)
        {
            audioSource.volume += .2f * Time.deltaTime / musicTransitionDuration;

            if (audioSource.volume >= currentVolume)
            {
                fadingIn = false;
                audioSource.volume = currentVolume;
            }
        }
        else if (fadingOut)
        {
            audioSource.volume -= currentVolume * Time.deltaTime / musicTransitionDuration;

            if (audioSource.volume <= 0)
            {
                if (changeMusic)
                {
                    fadingIn = true;
                    if (nextMusic != null) SetNewMusic(nextMusic);
                    audioSource.Play();
                    changeMusic = false;
                    nextMusic = null;
                }
                fadingOut = false;
                audioSource.volume = 0;
            }
        }
    }

    public void SetNewMusic(AudioClip audioClip)
    {
        audioSource.clip = audioClip;
    }

    /// <summary>
    /// Fades out the old audio and fades in the new one.
    /// </summary>
    /// <param name="audio">The new audio to be played</param>
    public void ChangeMusicSmoothly(AudioClip audio)
    {
        instance.fadingOut = true;
        instance.fadingIn = false;
        instance.changeMusic = true;
        instance.nextMusic = audio;
    }

    public void PlayMusicImmediatelyWithFadeIn(AudioClip audio)
    {
        audioSource.volume = 0;
        SetNewMusic(audio);
        audioSource.Play();
        instance.fadingOut = false;
        instance.fadingIn = true;
    }

    private void FadeOut()
    {
        animator.SetTrigger("fadeout");
    }

    private void FadeIn()
    {
        animator.SetTrigger("fadein");
    }

    public AudioClip GetAudioClipPlaying()
    {
        return audioSource.clip;
    }

    public void SetNewMusicVolume(float volume)
    {
        currentVolume = volume;
        SetAudioSourceVolume(volume);
    }

    private void SetAudioSourceVolume(float newVolume)
    {
        audioSource.volume = newVolume;
    }

    public void SetCurrentMusicVolumeToMax()
    {
        currentVolume = maxVolume;
        SetAudioSourceVolume(currentVolume);
    }

    public void SetMenuAudioVolume()
    {
        currentVolume *= 1/*.25f*/; //Increases volume by 25%
        SetAudioSourceVolume(currentVolume);
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
        instance.isMuted = mute;
    }
}
