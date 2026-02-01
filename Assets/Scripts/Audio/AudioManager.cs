using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Audio Source")]
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _sfxSource;

    [Header("Audio Clip")]
    public AudioClip mainMenuMusic;
    public AudioClip parkourMusic;

    private void Start()
    {
        _musicSource.clip = mainMenuMusic;
        _musicSource.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        _sfxSource.PlayOneShot(clip);
    }

    private void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.Play();
    }

}


