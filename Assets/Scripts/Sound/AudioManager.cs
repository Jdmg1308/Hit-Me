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
    public AudioClip alleywayMusic;
    public AudioClip sewerMusic;
    public AudioClip shopMusic;
    public AudioClip titleScreenMusic;
    public AudioClip tutorialMusic;
    public AudioClip punch;
    public AudioClip uppercut;
    public AudioClip footsteps;
    public AudioClip enemyHit;
    public AudioClip playerHit;
    public AudioClip jump;

    private Dictionary<string, AudioClip> levelMusic;

    private void Awake()
    {
        // Initialize level music dictionary
        levelMusic = new Dictionary<string, AudioClip>
        {
            // testing right now
            {"CARDS", alleywayMusic },
            {"GOON", sewerMusic },
            {"RANGED", shopMusic },
            {"MORERANGED", titleScreenMusic },
            {"GRAPPLE", tutorialMusic }
        };
    }

    // private void Start()
    //{
    //    musicSource.clip = titleScreenMusic;
    //    musicSource.Play();

    //}

    public void PlayLevelMusic(string levelName)
    {
        if (levelMusic.TryGetValue(levelName, out AudioClip clip))
        {
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("No music found for: " + levelName);
        }
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