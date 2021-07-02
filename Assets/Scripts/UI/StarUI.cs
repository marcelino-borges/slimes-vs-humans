using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarUI : MonoBehaviour
{
    public AudioClip activateSfx;
    private AudioSource audioSource;
    public ParticleSystem activationParticles;
    public Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        audioSource.volume = SoundManager.instance.CurrentVolume;
    }

    public void Activate()
    {
        animator.SetTrigger("activate");
    }

    /// <summary>
    /// Called from animation event
    /// </summary>
    private void PlayActivationSound()
    {
        if(activateSfx != null)
            audioSource.PlayOneShot(activateSfx);

        if (activationParticles != null)
            activationParticles.Play();
    }
}
