using UnityEngine.Audio;
using System;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    static string currentSong;

    public Music[] tracks;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            instance.Stop();
            return;
        }

        DontDestroyOnLoad(this);

        foreach(Music sound in tracks)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
        }
    }

    public void Play(string name)
    {

        Music s = Array.Find(tracks, sound => sound.name == currentSong);
        if (s != null)
        {
            s.source.Stop();
        }
        s = Array.Find(tracks, sound => sound.name == name);
        if (s == null || (s.loop && s.source.isPlaying)) { return; }
        s.source.volume = s.volume * DataManager.musicVolume;
        s.source.Play();
        currentSong = name;
    }

    public void Stop()
    {
        foreach (Music s in tracks)
        {
            s.source.Stop();
        }
    }

    public void Stop(string name)
    {
        Music s = Array.Find(tracks, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }

    public void SetPitch(string name, float pitch)
    {
        Music s = Array.Find(tracks, sound => sound.name == name);
        if (s != null)
        {
            s.source.pitch = pitch;
        }
    }

    public void SetVolume(float volume)
    {
        Music s = Array.Find(tracks, sound => sound.name == currentSong);
        if (s != null)
        {
            s.source.volume = volume;
        }
    }

    public void SetVolume(string name, float volume)
    {
        Music s = Array.Find(tracks, sound => sound.name == name);
        if (s != null)
        {
            s.source.volume = volume;
        }
    }
}
