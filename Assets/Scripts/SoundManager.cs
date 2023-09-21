using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;
using UnityEngine.Rendering;

public class SoundManager : MonoBehaviour
{
    public Sound[] sounds;
    // Start is called before the first frame update
    void Awake()
    {
        sounds = Resources.LoadAll<Sound>("Sounds/SO");
        foreach(Sound sound in sounds)
        {
            AudioSource new_source = gameObject.AddComponent<AudioSource>();
            sound.sources.Add(new_source);
            sound.sources[0].clip = sound.clip;
            sound.sources[0].volume = sound.volume;
            sound.sources[0].pitch = sound.pitch;
        }
    }

    public void play(string name, float pitch = default(float), float volume = default(float))
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (volume == default(float)) volume = 0.25f;
        if (pitch == default(float)) pitch = 1f;

        int i = 0;
        int ii = 0;

        foreach (var source in s.sources)
        {
            if (source.isPlaying)
            {
                i ++;
            }
            else
            {
                s.sources[ii].volume = volume;
                s.sources[ii].pitch = pitch;
                s.sources[ii].Play();
                break;
            }

            ii ++;
        }

        if (i - s.sources.Count == 0)
        {
            AudioSource new_source = gameObject.AddComponent<AudioSource>();
            s.sources.Add(new_source);
            s.sources[i].clip = s.clip;
            s.sources[i].volume = volume;
            s.sources[i].pitch = pitch;
            s.sources[i].Play();
        }
    }

    void OnApplicationQuit()
    {
        foreach (Sound sound in sounds)
        {
            sound.sources.Clear();
        }
    }
}
