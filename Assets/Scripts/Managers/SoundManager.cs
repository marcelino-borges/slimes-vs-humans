﻿using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    [HideInInspector]
    public AudioSource audioSource;
    private float currentVolume = 1f;
    public float CurrentVolume { get => isMuted ? 0 : currentVolume; set => currentVolume = value; }

    public float maxVolume = 1f;

    public AudioClip buttonClickSfx;
    public bool isMuted = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.mute = isMuted;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
    }

    public void PlaySound2D(AudioClip audio, bool abortIfPlaying = false)
    {
        if (abortIfPlaying)
        {
            audioSource.PlayOneShot(audio, CurrentVolume);
        } else
        {
            if (!audioSource.isPlaying)
                audioSource.PlayOneShot(audio, CurrentVolume);
        }
    }

    public void PlaySoundAtPosition(AudioClip audio, Vector3 position, float volume = -1f)
    {
        float vol = CurrentVolume;
        if (volume >= 0) vol = volume;        
        AudioSource.PlayClipAtPoint(audio, position, vol);
    }

    public void PlayButtonSfx()    
    {
        if(buttonClickSfx != null)
            PlaySound2D(buttonClickSfx);
    }

    public void Mute(bool mute)
    {
        audioSource.mute = mute;
        instance.isMuted = mute;
    }
}
