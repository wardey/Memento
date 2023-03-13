using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SongAtlas : MonoBehaviour
{

    public AudioClip[] songs;
    public static SongAtlas instance;
    public Dictionary<string, AudioClip> dictSongs = new Dictionary<string, AudioClip>();
    // Use this for initialization
    void Awake()
    {
        instance = this;
        foreach (AudioClip song in songs)
        {
            dictSongs.Add(song.name, song);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public AudioClip loadSong(string name)
    {
        if (dictSongs.ContainsKey(name))
        {
            return dictSongs[name];
        }
        else return songs[0];
    }

    public AudioClip loadSong(int index)
    {
        if (songs.Length > index)
        {
            return songs[index];
        }
        else return songs[0];
    }
}
