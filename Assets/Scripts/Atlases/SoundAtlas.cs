using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundAtlas : MonoBehaviour {

    public AudioClip[] sounds;
    public static SoundAtlas instance;
    public Dictionary<string, AudioClip> dictSounds = new Dictionary<string, AudioClip>();
    // Use this for initialization
    void Awake()
    {
        instance = this;
        foreach (AudioClip sound in sounds)
        {
            dictSounds.Add(sound.name, sound);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public AudioClip loadSound(string name)
    {
        if (dictSounds.ContainsKey(name))
        {
            return dictSounds[name];
        }
        else return null;
    }
}
