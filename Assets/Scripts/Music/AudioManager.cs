using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    public string CurrentlyPlayingBGM;
    public Sound[] sounds;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;   
        }
        DontDestroyOnLoad(this.gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();

            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    private void Start()
    {
        PlaySound("BGM_title");
    }

    public void PlaySound(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Uhoh, can't find " + name + " to play");
            return;
        }

        switch (s.audioPurpose)
        {
            case 0:
                s.source.volume = PlayerPrefs.GetFloat("BGM", 5f) / 10;
                CurrentlyPlayingBGM = s.name;
                break;

            case 1:
                s.source.volume = PlayerPrefs.GetFloat("UI", 5f) / 10;
                break;

            case 2:
                s.source.volume = PlayerPrefs.GetFloat("InGame", 5f) / 10;
                break;

            default:
                break;
        }

        s.source.Play();
    }

    public void StopPlaying(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.Log("Uhoh, can't find " + name + " to play");
            return;
        }

        s.source.Stop();
    }
}
