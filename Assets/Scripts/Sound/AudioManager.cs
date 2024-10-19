using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("------------- Audio Sourcce ---------")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource FootstepsSource;

    [Header("------------ Audio Clip -----------")]
    public AudioClip background;
    public AudioClip punch;
    public AudioClip uppercut;
    public AudioClip footsteps;
    public AudioClip enemyHit;
    public AudioClip playerHit;
    public AudioClip jump;

    private void Start()
    {
        musicSource.clip = background;
        musicSource.Play();

    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayFootsteps(AudioClip clip)
    {
        if (!FootstepsSource.enabled)
        {
            FootstepsSource.enabled = true;
            FootstepsSource.clip = clip;
            FootstepsSource.loop = true;
            FootstepsSource.Play();
        }

    }
    
    public void StopFootsteps()
    {
        if (FootstepsSource.enabled)
        {
            FootstepsSource.Stop();
            FootstepsSource.enabled = false;
        }
    }

}