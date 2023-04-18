using UnityEngine.Audio;
using System;
using UnityEngine;

public class VehicleAudioManager : MonoBehaviour
{
    public static VehicleAudioManager instance;

    public Sound[] sounds;

    private void Awake()
    {
        instance = this;

        foreach(Sound sound in sounds)
        {
            sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;

            sound.source.volume = sound.volume;
            sound.source.pitch = sound.pitch;
            sound.source.loop = sound.loop;
            sound.source.spatialBlend = 1;
            sound.source.rolloffMode = AudioRolloffMode.Linear;
            sound.source.dopplerLevel = 0.5f;
            sound.source.maxDistance = 60;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null || (s.loop && s.source.isPlaying)) { return; }
        s.source.volume = s.volume * DataManager.soundVolume;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.Stop();
        }
    }

    public void SetPitch(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s != null)
        {
            s.source.pitch = pitch;
        }
    }
}
